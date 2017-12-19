# CrpcVisualiser
A visualisation tool for Crestron Media Player module developers.

This is the source code for a CRPC Visualiser module.  When developing Media Player modules, it's helpful to see the conversation between the Media Player Router object in SIMPL and the SIMPL# implementation of your driver.

This conversation can be quite chatty and contains a lot of information that can be hard to tease out.

The CRPC Visualiser classes and SIMPL+ wrapper provide a way to visualise this conversation in a standard web browser.

The CRPC messages are passed into the Visualiser module, which then generates an XML file in the Crestron procoessor's internal web server.  This XML file can then be opened in a web browser.

The project includes an XSLT file which will transorm the XML into a more human readable form.
