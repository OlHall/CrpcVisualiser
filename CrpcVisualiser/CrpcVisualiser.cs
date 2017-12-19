using System;
using System.Collections.Generic;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Newtonsoft.Json;

namespace Ultamation.SimplSharp.Tools
{
    /// <summary>
    /// A helper class for cpaturing and visualising CPRC messages
    /// </summary>
    public class CrpcVisualiser
    {
        private const int VIS_GAP_TIME = 1000;

        private CCriticalSection _eventsLock;
        private CCriticalSection _msgLock;
        private CTimer _gapTimer;
        private List<CrpcEvent> _events;
        private string _messageFromMp;
        private string _messageFromDrv;

        /// <summary>
        /// SIMPL+ can only execute the default constructor. If you have variables that require initialization, please
        /// use an Initialize method
        /// </summary>
        public CrpcVisualiser()
        {
            _eventsLock = new CCriticalSection();
            _msgLock = new CCriticalSection();
            _events = new List<CrpcEvent>();
            _gapTimer = new CTimer(OnGapTimerExpired, null, VIS_GAP_TIME, VIS_GAP_TIME);
        }

        /// <summary>
        /// Extract the SG version information from the event history
        /// </summary>
        private string From
        {
            get
            {
                try
                {

                    CrpcEvent SGId = _events.Find(e => (e.EventType == CrpcEvent.Type.CallMethod) && (e.MethodObject == "Crpc") && (e.MethodSig == "Register"));
                    if (SGId != null)
                    {
                        string[] sgInfo = ((string)SGId.parameters["name"]).Split('_');
                        string[] sgVer = sgInfo[0].Split('=');
                        return sgVer[1];
                    }
                    else
                    {
                        return "Unknown!";
                    }
                }
                catch
                {
                    return "Unknown!";
                }
            }
        }

        /// <summary>
        /// Extract the driver's class name from the event list
        /// </summary>
        private string To
        {
            get
            {
                try
                {
                    CrpcEvent SGId = _events.Find(e => (e.EventType == CrpcEvent.Type.CallMethod) && (e.MethodObject == "Crpc") && (e.MethodSig == "GetObjects") && ( e.ResultEvent != null ));
                    if( SGId != null )
                    {
                        if (SGId.ResultEvent.resultDict != null)
                        {
                            // return SGId.ResultEvent.resultDict["objects"].objectInst[0].name;
                            if (SGId.ResultEvent.resultDict.ContainsKey("objects"))
                            {
                                Dictionary<string,object> objs = JsonConvert.DeserializeObject<Dictionary<string,object>>(SGId.ResultEvent.resultDict["objects"].ToString());
                                if (objs.ContainsKey("object"))
                                {
                                    object[] objArray = JsonConvert.DeserializeObject<object[]>(objs["object"].ToString());
                                    if ((objArray != null) && (objArray.Length > 0))
                                    {
                                        Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(objArray[0].ToString());
                                        if ((dict != null) && (dict.ContainsKey("name")))
                                            return dict["name"].ToString();
                                        return "name?";
                                    }
                                    return "Objects.Count = 0?";
                                }
                                return "Objects.object?";
                            }
                            return "Objects?";
                        }
                        return "result?";
                    }
                    return "GetObjects()?";
                }
                catch( Exception e )
                {
                    return "Error! " +  e.Message;
                }
            }
        }

        /// <summary>
        /// Timer callback that fires for every second elapsed without a message in or out
        /// </summary>
        /// <param name="usrObj">unused</param>
        private void OnGapTimerExpired(object usrObj)
        {
            _eventsLock.Enter();
            try
            {
                _messageFromMp = string.Empty;
                _messageFromDrv = string.Empty;
                if (_events.Count > 1)
                {
                    CrpcEvent lastEvent = _events[_events.Count-1];
                    if( lastEvent.EventType == CrpcEvent.Type.Gap )
                    {
                        lastEvent.GapSecs++;
                    }
                    else
                    {
                        _events.Add( new CrpcEvent()
                        {
                            EventType = CrpcEvent.Type.Gap,
                            GapSecs = 1
                        });
                    }
                }
            }
            finally
            {
                _eventsLock.Leave();
            }
        }

        /// <summary>
        /// Initialise the event list (clear it) and add an initial bookmark as a header
        /// </summary>
        /// <param name="mark">An initial bookmark to place at the top of the visualisation page</param>
        public void Initialise(SimplSharpString mark)
        {
            _eventsLock.Enter();
            try
            {
                _events.Clear();
                if (!string.IsNullOrEmpty(mark.ToString()))
                {
                    _events.Add(new CrpcEvent()
                    {
                        EventType = CrpcEvent.Type.Mark,
                        Description = mark.ToString()
                    });
                }
            }
            finally
            {
                _eventsLock.Leave();
            }
        }

        /// <summary>
        /// Write out the event XML file to the given filepath
        /// </summary>
        /// <param name="fname">the file path e.g. \\HTML\\crpc_vis.xml</param>
        public void Write(SimplSharpString fname)
        {
            _eventsLock.Enter();
            try
            {
                // Pre-process the event list to match up results with requests
                foreach (CrpcEvent evt in _events)
                {
                    evt.Ignore = false;
                    if (((evt.EventType == CrpcEvent.Type.Result) || (evt.EventType == CrpcEvent.Type.CrpcError)) && (evt.id > 1))
                    {
                        CrpcEvent reqEvt = _events.Find(e => ( e.id == evt.id ) && ( e.EventType != CrpcEvent.Type.Result ) && ( e.EventType != CrpcEvent.Type.CrpcError ));
                        if (reqEvt != null)
                        {
                            evt.Ignore = true;
                            reqEvt.ResultEvent = evt;
                        }                        
                    }
                }

                using (FileStream fs = new FileStream(fname.ToString(), FileMode.Create))
                {
                    fs.Write("<?xml version=\"1.0\"?>\n", Encoding.UTF8);
                    fs.Write("<?xml-stylesheet type=\"text/xsl\" href=\"crpc.xsl\"?>\n", Encoding.UTF8);

                    fs.Write("<crpcCapture>\n", Encoding.UTF8);
                    fs.Write(" <crpcSummary>\n", Encoding.UTF8);
                    fs.Write(string.Format("  <from>{0}</from>\n", From), Encoding.UTF8);
                    fs.Write(string.Format("  <to>{0}</to>\n", To), Encoding.UTF8);
                    fs.Write(" </crpcSummary>\n", Encoding.UTF8);

                    fs.Write(" <crpcEvents>\n", Encoding.UTF8);
                    foreach (CrpcEvent evt in _events)
                    {
                        if (!evt.Ignore)
                        {
                            fs.Write(string.Format("  <event type=\"{0}\" id=\"{1}\">\n", evt.EventType.ToString(), evt.id), Encoding.UTF8);
                            if (!string.IsNullOrEmpty(evt.Description))
                                fs.Write(string.Format("   <description><![CDATA[{0}]]></description>\n", evt.Description), Encoding.UTF8);
                            if (!string.IsNullOrEmpty(evt.MethodObject))
                                fs.Write(string.Format("   <object><![CDATA[{0}]]></object>\n", evt.MethodObject), Encoding.UTF8);
                            if (!string.IsNullOrEmpty(evt.MethodSig))
                                fs.Write(string.Format("   <method><![CDATA[{0}]]></method>\n", evt.MethodSig), Encoding.UTF8);
                            if (!string.IsNullOrEmpty(evt.Detail))
                                fs.Write(string.Format("   <detail><![CDATA[{0}]]></detail>\n", evt.Detail), Encoding.UTF8);
                            if (evt.GapSecs > 0)
                                fs.Write(string.Format("   <gap>{0}</gap>\n", evt.GapSecs), Encoding.UTF8);
                            fs.Write(string.Format("   <original><![CDATA[{0}]]></original>\n", evt.OriginalJson), Encoding.UTF8);

                            // Do we have a result event too?
                            if (evt.ResultEvent != null)
                            {
                                fs.Write(string.Format("   <response type=\"{0}\">\n", evt.ResultEvent.EventType.ToString()), Encoding.UTF8);
                                if (!string.IsNullOrEmpty(evt.ResultEvent.Description))
                                    fs.Write(string.Format("    <description><![CDATA[{0}]]></description>\n", evt.ResultEvent.Description), Encoding.UTF8);
                                if (!string.IsNullOrEmpty(evt.ResultEvent.Detail))
                                    fs.Write(string.Format("    <detail><![CDATA[{0}]]></detail>\n", evt.ResultEvent.Detail), Encoding.UTF8);
                                fs.Write(string.Format("    <original><![CDATA[{0}]]></original>\n", evt.ResultEvent.OriginalJson), Encoding.UTF8);
                                fs.Write("   </response>\n", Encoding.UTF8);
                            }

                            fs.Write(string.Format("  </event>\n"), Encoding.UTF8);
                        }
                    }
                    fs.Write(" </crpcEvents>\n", Encoding.UTF8);
                    fs.Write("</crpcCapture>\n", Encoding.UTF8);
                    fs.Close();
                }
            }
            finally
            {
                _eventsLock.Leave();
            }
        }

        /// <summary>
        /// The SIMPL+ methof for adding a bookmark to the event list
        /// </summary>
        /// <param name="mark"></param>
        public void AddBookmark(SimplSharpString mark)
        {
            _eventsLock.Enter();
            try
            {
                _gapTimer.Reset(VIS_GAP_TIME, VIS_GAP_TIME);

                _events.Add(new CrpcEvent()
                    {
                        EventType = CrpcEvent.Type.Mark,
                        Description = mark.ToString()
                    });
            }
            finally
            {
                _eventsLock.Leave();
            }
        }

        /// <summary>
        /// Handle messages for from the Media Player SG object
        /// </summary>
        /// <param name="msgMp">An inbound CRPC message (including header and potentially incomplete JSON objects</param>
        public void ProcessCrpcMessageFromMp(SimplSharpString msgMp)
        {
            // First 8 characters are some sort of identifier
            string msgTrim = msgMp.ToString().Substring(8);
            int msgPreamble = Convert.ToInt32(msgMp.ToString().Substring(0, 8), 16);
#if DEBUG
            CrestronConsole.PrintLine("-->:{0:x8}",msgPreamble);
#endif
            List<string> jsonObjects;
            _msgLock.Enter();
            try
            {
                jsonObjects = SeparateJsonObjects(_messageFromMp + msgTrim, out _messageFromMp);
#if DEBUG
                if (jsonObjects.Count > 0)
                    CrestronConsole.PrintLine("Processing {0} objects from MP", jsonObjects.Count);
#endif
                foreach (string json in jsonObjects)
                {
                    ProcessCrpcMessage(json);
                }
            }
            catch (Exception e)
            {
#if DEBUG
                CrestronConsole.PrintLine("ProcessCrpcMessageFromMp blew up when splitting JSON objects: " + e.Message);
#endif
                _messageFromMp = string.Empty;
            }
            finally
            {
                _msgLock.Leave();
            }
        }

        /// <summary>
        /// Handle messages for from the driver implementing IMediaPlayer
        /// </summary>
        /// <param name="msgDrv">An inbound CRPC message (including header and potentially incomplete JSON objects</param>
        public void ProcessCrpcMessageFromDrv(SimplSharpString msgDrv)
        {
            // First 8 characters are some sort of identifier
            string msgTrim = msgDrv.ToString().Substring(8);
            int msgPreamble = Convert.ToInt32(msgDrv.ToString().Substring(0, 8), 16);
#if DEBUG
            CrestronConsole.PrintLine("<--:{0:x8}", msgPreamble);
#endif

            List<string> jsonObjects;
            _msgLock.Enter();
            try
            {
                jsonObjects = SeparateJsonObjects(_messageFromDrv + msgTrim, out _messageFromDrv);
#if DEBUG
                if( jsonObjects.Count > 0 )
                    CrestronConsole.PrintLine("Processing {0} objects from Driver", jsonObjects.Count);
#endif
                foreach (string json in jsonObjects)
                {
                    ProcessCrpcMessage(json);
                }
            }
            catch (Exception e)
            {
#if DEBUG
                CrestronConsole.PrintLine("ProcessCrpcMessageFromDrv blew up when splitting JSON objects: " + e.Message);
#endif
                _messageFromDrv = string.Empty;
            }
            finally
            {
                _msgLock.Leave();
            }
        }

        /// <summary>
        /// Given a string which may contain one or more JSON objects, break them into individual JSON objects and return whatever's left
        /// Probably an incomplete/partial JSON object
        /// </summary>
        /// <param name="collection">The input string</param>
        /// <param name="remainder">The output remainder</param>
        /// <returns>A collection of individual strings which, hopefully each represent a complete JSON object</returns>
        private List<string> SeparateJsonObjects(string collection, out string remainder)
        {
            List<string> objs = new List<string>();

            int startIdx = 0;
            int currentIdx = 0;
            int braceCount = 0;
            bool inQuotes = false;

            do
            {
                char curCh = collection[currentIdx];
                if (inQuotes)
                {
                    if (curCh == '"')
                        inQuotes = false;
                }
                else
                {
                    switch (curCh)
                    {
                        case '"':
                            {
                                inQuotes = true;
                                break;
                            }
                        case '{':
                            {
                                if (braceCount == 0)
                                    startIdx = currentIdx;
                                braceCount++;
                                break;
                            }
                        case '}':
                            {
                                braceCount--;
                                if (braceCount == 0)
                                {
                                    objs.Add(collection.Substring(startIdx, currentIdx - startIdx + 1));
                                    startIdx = currentIdx + 1;
                                }
                                break;
                            }
                    }
                }

                // Move on to the next character
                currentIdx++;
            } while (currentIdx < collection.Length);

            if (startIdx < collection.Length)
                remainder = collection.Substring(startIdx);
            else
                remainder = string.Empty;

            return objs;
        }

        /// <summary>
        /// Process a single JSON object and populate the event list appropriately
        /// </summary>
        /// <param name="json">The incoming message (a single object)</param>
        /// <returns>true if all goes well, otherwise false</returns>
        private bool ProcessCrpcMessage(string json)
        {
            bool bRet = true;
            CrpcEvent newEvt = null;
            try
            {
                newEvt = JsonConvert.DeserializeObject<CrpcEvent>(json,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });

                newEvt.OriginalJson = json;

                // Handle result being EITHER bool or Dict<str,obj>
                try
                {
                    newEvt.resultDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(newEvt.result.ToString());
                }
                catch
                {
                    newEvt.resultDict = null;
                }
                
                if (!string.IsNullOrEmpty(newEvt.method))
                {
                    string[] methodParts = newEvt.method.Split('.');
                    if (methodParts.Length >= 2)
                    {
                        newEvt.MethodObject = methodParts[0];
                        newEvt.MethodSig = methodParts[1];
                    }
                    else
                    {
                        newEvt.MethodSig = newEvt.method;
                    }

                    if (newEvt.MethodSig.Contains("RegisterEvent"))
                    {
                        newEvt.EventType = CrpcEvent.Type.RegisterEvent;
                        newEvt.Description = newEvt.parameters["ev"].ToString();
                    }
                    else if (newEvt.MethodSig.Contains("Event"))
                    {
                        newEvt.EventType = CrpcEvent.Type.Event;
                        newEvt.Description = newEvt.parameters["ev"].ToString();
                        if (newEvt.parameters.ContainsKey("parameters"))
                            newEvt.Detail = newEvt.parameters["parameters"].ToString();
                        else
                            newEvt.Detail = "No payload";

                    }
                    else if (newEvt.MethodSig.Contains("GetProperty"))
                    {
                        newEvt.EventType = CrpcEvent.Type.GetProperty;
                        newEvt.Description = newEvt.parameters["propName"].ToString();
                    }
                    else
                    {
                        newEvt.EventType = CrpcEvent.Type.CallMethod;
                        newEvt.Description = "Method call - see MethodSig";
                    }
                }
                else if (newEvt.error != null)
                {
                    newEvt.EventType = CrpcEvent.Type.CrpcError;
                    newEvt.Description = newEvt.error.message;
                }
                else
                {
                    newEvt.EventType = CrpcEvent.Type.Result;

                    string prop, val;
                    newEvt.GetResultPropertyValue(out prop, out val);

                    newEvt.Description = prop;
                    newEvt.Detail = val;
                }
            }
            catch (Exception e)
            {
                bRet = false;

#if DEBUG
                CrestronConsole.PrintLine("CrpcVisualiser Process: Exception: " + e.Message);
                CrestronConsole.PrintLine(": " + json);
#endif
                newEvt = new CrpcEvent()
                {
                    EventType = CrpcEvent.Type.Exception,
                    Description = e.GetType().ToString() + " > " + e.Message + ": " + json,
                    Detail = e.ToString(),
                    OriginalJson = json
                };
            }

            if (newEvt != null)
            {
                _eventsLock.Enter();
                try
                {
#if DEBUG
                    CrestronConsole.PrintLine("Adding Event: [{0}] {1}", newEvt.id, newEvt.EventType);
#endif
                    _gapTimer.Reset(VIS_GAP_TIME, VIS_GAP_TIME);
                    _events.Add(newEvt);
                }
                finally
                {
                    _eventsLock.Leave();
                }
            }
            else
            {
#if DEBUG
                CrestronConsole.PrintLine("FAILED Adding Event!");
#endif
            }

            return bRet;
        }

    }
}
