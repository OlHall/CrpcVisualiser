using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ultamation.SimplSharp.Tools
{
    /// <summary>
    /// Deserialising the CRPC Error structure
    /// </summary>
    public class CrpcError
    {
        public int code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    /// <summary>
    /// A general CRPC object for deserialising, plus additional properties
    /// for generating the visualisation XML
    /// Far from efficient!
    /// </summary>
    public class CrpcEvent
    {
        public enum Type
        {
            CallMethod,
            RegisterEvent,
            GetProperty,
            Event,
            CrpcError,
            Result,
            Mark,
            Gap,
            Exception
        }

        public string jsonrpc { get; set; }
        public int id { get; set; }
        [JsonProperty(PropertyName = "params")]
        public Dictionary<string,object> parameters { get; set; }
        public JObject result { get; set; }
        public CrpcError error { get; set; }
        public string method { get; set; }

        // Extension properties
        public Type EventType { get; set; }
        public int GapSecs { get; set; }
        public string MethodObject { get; set; }
        public string MethodSig { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public string OriginalJson { get; set; }
        public Dictionary<string, object> resultDict { get; set; }
        public bool Ignore { get; set; }
        public CrpcEvent ResultEvent { get; set; }

        /// <summary>
        /// Extract the first result name/value pair if possible
        /// or assume the result is boolean - a big assumption!
        /// </summary>
        /// <param name="prop">The property name</param>
        /// <param name="val">The porerty value</param>
        public void GetResultPropertyValue(out string prop, out string val)
        {
            if (resultDict == null)
            {
                prop = "Boolean";
                val = result.ToString();
            }
            else
            {
                if ((resultDict != null) && (resultDict.Count > 0))
                {
                    IEnumerator enumerator = resultDict.Keys.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        prop = (string)enumerator.Current;
                        if (resultDict[prop] != null)
                            val = resultDict[prop].ToString();
                        else
                            val = "null";
                    }
                    else
                    {
                        prop = "No Property";
                        val = "Undefined";
                    }
                }
                else
                {
                    prop = "No Result";
                    val = "Undefined";
                }
            }
        }
    }
}
