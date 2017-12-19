<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template match="/">
		<html>
			<style>
				* {
				-webkit-box-sizing: border-box;
				-moz-box-sizing: border-box;
				box-sizing: border-box;
				}

				body
				{
				max-width: 800px;
				margin-left: auto;
				margin-right: auto;
				margin-top: 16px;
				margin-bottom: 60px;
				font-family: "Consolas";
				}

				.objTitle
				{
				width: 200px;
				padding: 8px;
				border: solid black 2px;
				background-color: #F0F0F0;
				text-align: center;
				box-shadow: 2px 4px 4px rgba(0, 0, 0, 0.2);
				}

				.event
				{
				clear: both;
				position: relative;
				margin: 0px 80px;
				}

				.solidLines
				{
				border: 2px solid black;
				border-top: 0px;
				border-bottom: 0px;
				margin: 0px 19px;
				height: 20px;
				}

				.dottedLines
				{
				border: 2px dotted black;
				border-top: 0px;
				border-bottom: 0px;
				margin: 0px 19px;
				height: 20px;
				}

				.objTop
				{
				width: 20px;
				height: 20px;
				border: 2px solid black;
				border-bottom: 0px;
				background-color: white;
				margin: 0px 10px;
				box-shadow: 2px 4px 4px rgba(0, 0, 0, 0.2);
				}

				.objMid
				{
				position: absolute;
				width: 20px;
				height: 50px;
				border: 2px solid black;
				border-top: 0px;
				border-bottom: 0px;
				background-color: white;
				margin: 0px 10px;
				box-shadow: 2px 4px 4px rgba(0, 0, 0, 0.2);
				}

				.objBottom
				{
				width: 20px;
				height: 20px;
				border: 2px solid black;
				border-top: 0px;
				background-color: white;
				margin: 0px 10px;
				box-shadow: 2px 4px 4px rgba(0, 0, 0, 0.2);
				}

				header {
				height: 80px;
				text-align: center;
				}

				footer {
				position: fixed;
				bottom: 0;
				left: 0;
				background-color: #F0F0F0;
				width: 100%;
				text-align: center;
				}

				.markEvent
				{
				margin: 0px 40px;
				border: 2px solid #008000;
				background-color: #F0FFF0;
				color: #004000;
				text-align: center;
				padding: 2px;
				box-shadow: 2px 4px 4px rgba(0, 0, 0, 0.2);
				}

				.crpcErrorEvent
				{
				margin: 0px 40px;
				border: 2px solid #800000;
				background-color: #FFE0E0;
				color: #800000;
				text-align: center;
				padding: 2px;
				box-shadow: 2px 4px 4px rgba(0, 0, 0, 0.2);
				}

				.exceptionEvent
				{
				border: 2px solid #800000;
				background-color: #FFE0E0;
				color: #800000;
				text-align: center;
				padding: 2px;
				box-shadow: 2px 4px 4px rgba(0, 0, 0, 0.2);
				}

				.undefinedEvent
				{
				border: 2px solid #FFA000;
				background-color: #FFE0B0;
				color: #000000;
				text-align: center;
				padding: 2px;
				box-shadow: 2px 4px 4px rgba(0, 0, 0, 0.2);
				}

				.gapEvent
				{
				width: 40px;
				color: #0000FF;
				text-align: center;
				}

				.rpcInfo
				{
				position: relative;
				margin: 0px 30px;
				}

				.rpcArrowLine
				{
				position: absolute;
				top: 24px;
				height: 3px;
				width: 100%;
				}

				.rpcArrowHead
				{
				position: absolute;
				width: 21px;
				height: 15px;
				top: 18px;
				}

				.rpcEvent
				{
				white-space: nowrap;
				color: #000000;
				text-align: center;
				height:25px;
				<!--
				display: -webkit-flex;
				display:         flex;
				-webkit-align-items: center;
				align-items: center;
				-webkit-justify-content: center;
				justify-content: center;
				-->
				margin: 0px 24px;
				overflow: hidden;
				text-overflow: ellipsis;
				}

				.rpcTooltip
				{
				visibility: hidden;
				width: 800px;
				border: 2px solid #000000;
				border-radius: 3px;
				background-color: #FFF8E0;
				color: #000000;
				word-wrap: break-word;
				text-align:center;
				padding: 5px;
				box-shadow: 2px 4px 4px rgba(0, 0, 0, 0.2);

				position: absolute;
				z-index: 1;
				}

				.rpcInfo:hover .rpcTooltip
				{
				visibility: visible;
				}

				.crpcErrorEvent:hover .rpcTooltip
				{
				visibility: visible;
				}
			</style>
			<body>
				<header>
					<h1>CRPC Visualiser</h1>
				</header>

				<div>
					<div style="float: left;" class="objTitle">
						MP: <xsl:value-of select="/crpcCapture/crpcSummary/from"/>
					</div>
					<div style="float: right;" class="objTitle">
						<xsl:value-of select="/crpcCapture/crpcSummary/to"/>
					</div>

					<div class="event">
						<div class="solidLines" />
						<div class="objTop" style="float: left;"/>
						<div class="objTop" style="float: right;"/>
					</div>

					<xsl:for-each select="crpcCapture/crpcEvents/event">
						<xsl:choose>
							<xsl:when test="@type = 'Mark'">
								<div class="event" style="height: 50px;">
									<div class="objMid" style="left: 0px;"/>
									<div class="objMid" style="right: 0px;"/>
									<div class="markEvent">
										<xsl:value-of select="description"/>
									</div>
								</div>
							</xsl:when>

							<xsl:when test="@type = 'Gap'">
								<div class="event">
									<div class="objBottom" style="float: left;"/>
									<div class="objBottom" style="float: right;"/>
									<div class="dottedLines" style="clear: both;"/>
									<div class="gapEvent" style="float: left;">
										<xsl:value-of select="gap"/>s
									</div>
									<div class="gapEvent" style="float: right;">
										<xsl:value-of select="gap"/>s
									</div>
									<div class="dottedLines" style="clear: both;"/>
									<div class="objTop" style="float: left;"/>
									<div class="objTop" style="float: right;"/>
								</div>
							</xsl:when>

							<xsl:when test="@type = 'CallMethod'">
								<div class="event" style="height: 50px;">
									<div class="objMid" style="left: 0px;"/>
									<div class="objMid" style="right: 0px;"/>
									<div class="rpcInfo">
										<span class="rpcTooltip">
											<xsl:value-of select="original"/>
										</span>
										<svg class="rpcArrowLine">
											<line x1="0" y1="1" x2="100%" y2="1" style="stroke: rgb(0,128,0); stroke-width: 2;" />
										</svg>
										<svg class="rpcArrowHead" style="right: 0px;">
											<polygon points="0,0 20,7 0,14" style="fill: rgb(0,128,0);"/>
										</svg>
										<div class="rpcEvent" style="float: left;">
											<xsl:value-of select="object"/>
										</div>
										<div class="rpcEvent">
											<xsl:value-of select="method"/>()
										</div>
									</div>
								</div>

								<xsl:if test="response">
									<xsl:choose>
										<xsl:when test="response/@type = 'Result'">
											<div class="event" style="height: 50px;">
												<div class="objMid" style="left: 0px;"/>
												<div class="objMid" style="right: 0px;"/>
												<div class="rpcInfo">
													<span class="rpcTooltip">
														<xsl:value-of select="response/original"/>
													</span>
													<svg class="rpcArrowLine">
														<line x1="0" y1="1" x2="100%" y2="1" style="stroke: rgb(0,0,192); stroke-width: 2;" />
													</svg>
													<svg class="rpcArrowHead" style="left: 0px;">
														<polygon points="0,7 20,0 20,14" style="fill: rgb(0,0,192);"/>
													</svg>
													<div class="rpcEvent">
														<xsl:value-of select="response/description"/> = <xsl:value-of select="response/detail"/>
													</div>
												</div>
											</div>
										</xsl:when>
										<xsl:otherwise>
											<div class="event" style="height: 50px;">
												<div class="objMid" style="left: 0px;"/>
												<div class="objMid" style="right: 0px;"/>
												<div class="crpcErrorEventt">
													<span class="rpcTooltip">
														<xsl:value-of select="original"/>
													</span>
													<xsl:value-of select="response/description"/>
												</div>
											</div>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:if>
							</xsl:when>

							<xsl:when test="@type = 'RegisterEvent'">
								<div class="event" style="height: 50px;">
									<div class="objMid" style="left: 0px;"/>
									<div class="objMid" style="right: 0px;"/>
									<div class="rpcInfo">
										<span class="rpcTooltip">
											<xsl:value-of select="original"/>
										</span>
										<svg class="rpcArrowLine">
											<line x1="0" y1="1" x2="100%" y2="1" style="stroke: rgb(0,128,0); stroke-width: 2;" />
										</svg>
										<svg class="rpcArrowHead" style="right: 0px;">
											<polygon points="0,0 20,7 0,14" style="fill: rgb(0,128,0);"/>
										</svg>
										<div class="rpcEvent" style="float: left;">
											<xsl:value-of select="object"/>
										</div>
										<div class="rpcEvent">
											<xsl:value-of select="method"/>(<xsl:value-of select="description"/>)
										</div>
									</div>
								</div>

								<xsl:if test="response">
									<xsl:choose>
										<xsl:when test="response/@type = 'Result'">
											<div class="event" style="height: 50px;">
												<div class="objMid" style="left: 0px;"/>
												<div class="objMid" style="right: 0px;"/>
												<div class="rpcInfo">
													<span class="rpcTooltip">
														<xsl:value-of select="response/original"/>
													</span>
													<svg class="rpcArrowLine">
														<line x1="0" y1="1" x2="100%" y2="1" style="stroke: rgb(0,0,192); stroke-width: 2;" />
													</svg>
													<svg class="rpcArrowHead" style="left: 0px;">
														<polygon points="0,7 20,0 20,14" style="fill: rgb(0,0,192);"/>
													</svg>
													<div class="rpcEvent">
														<xsl:value-of select="response/description"/> = <xsl:value-of select="response/detail"/>
													</div>
												</div>
											</div>
										</xsl:when>
										<xsl:otherwise>
											<div class="event" style="height: 50px;">
												<div class="objMid" style="left: 0px;"/>
												<div class="objMid" style="right: 0px;"/>
												<div class="crpcErrorEvent">
													<span class="rpcTooltip">
														<xsl:value-of select="original"/>
													</span>
													<xsl:value-of select="response/description"/>
												</div>
											</div>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:if>
							</xsl:when>

							<xsl:when test="@type = 'GetProperty'">
								<div class="event" style="height: 50px;">
									<div class="objMid" style="left: 0px;"/>
									<div class="objMid" style="right: 0px;"/>
									<div class="rpcInfo">
										<span class="rpcTooltip">
											<xsl:value-of select="original"/>
										</span>
										<svg class="rpcArrowLine">
											<line x1="0" y1="1" x2="100%" y2="1" style="stroke: rgb(0,128,0); stroke-width: 2;" />
										</svg>
										<svg class="rpcArrowHead" style="right: 0px;">
											<polygon points="0,0 20,7 0,14" style="fill: rgb(0,128,0);"/>
										</svg>
										<div class="rpcEvent" style="float: left;">
											<xsl:value-of select="object"/>
										</div>
										<div class="rpcEvent">
											<xsl:value-of select="method"/>(<xsl:value-of select="description"/>)
										</div>
									</div>
								</div>

								<xsl:if test="response">
									<xsl:choose>
										<xsl:when test="response/@type = 'Result'">
											<div class="event" style="height: 50px;">
												<div class="objMid" style="left: 0px;"/>
												<div class="objMid" style="right: 0px;"/>
												<div class="rpcInfo">
													<span class="rpcTooltip">
														<xsl:value-of select="response/original"/>
													</span>
													<svg class="rpcArrowLine">
														<line x1="0" y1="1" x2="100%" y2="1" style="stroke: rgb(0,0,192); stroke-width: 2;" />
													</svg>
													<svg class="rpcArrowHead" style="left: 0px;">
														<polygon points="0,7 20,0 20,14" style="fill: rgb(0,0,192);"/>
													</svg>
													<div class="rpcEvent">
														<xsl:value-of select="response/description"/> = <xsl:value-of select="response/detail"/>
													</div>
												</div>
											</div>
										</xsl:when>
										<xsl:otherwise>
											<div class="event" style="height: 50px;">
												<div class="objMid" style="left: 0px;"/>
												<div class="objMid" style="right: 0px;"/>
												<div class="crpcErrorEvent">
													<span class="rpcTooltip">
														<xsl:value-of select="original"/>
													</span>
													<xsl:value-of select="response/description"/>
												</div>
											</div>
										</xsl:otherwise>
									</xsl:choose>
								</xsl:if>

							</xsl:when>

							<xsl:when test="@type = 'Event'">
								<div class="event" style="height: 50px;">
									<div class="objMid" style="left: 0px;"/>
									<div class="objMid" style="right: 0px;"/>
									<div class="rpcInfo">
										<span class="rpcTooltip">
											<xsl:value-of select="original"/>
										</span>
										<svg class="rpcArrowLine">
											<line x1="0" y1="1" x2="100%" y2="1" style="stroke: rgb(192,0,0); stroke-width:2;" stroke-dasharray="5,5" />
										</svg>
										<svg class="rpcArrowHead" style="left: 0px;">
											<line x1="0" y1="7" x2="20" y2="0" style="stroke: rgb(192,0,0); stroke-width: 2;" />
											<line x1="0" y1="7" x2="20" y2="14" style="stroke: rgb(192,0,0); stroke-width: 2;" />
										</svg>
										<div class="rpcEvent" style="float: right;">
											<xsl:value-of select="object"/>
										</div>
										<div class="rpcEvent">
											Event:<xsl:value-of select="description"/>
										</div>
										<div class="rpcEvent">
											<xsl:value-of select="detail"/>
										</div>
									</div>
								</div>
							</xsl:when>

							<!-- An orphaned result? Shouldn't happen? -->
							<xsl:when test="@type = 'Result'">
								<div class="event" style="height: 50px;">
									<div class="objMid" style="left: 0px;"/>
									<div class="objMid" style="right: 0px;"/>
									<div class="rpcInfo">
										<span class="rpcTooltip">
											<xsl:value-of select="original"/>
										</span>
										<svg class="rpcArrowLine">
											<line x1="0" y1="1" x2="100%" y2="1" style="stroke: rgb(255,170,0); stroke-width:2;" stroke-dasharray="5,5" />
										</svg>
										<svg class="rpcArrowHead" style="left: 0px;">
											<line x1="0" y1="7" x2="20" y2="0" style="stroke: rgb(255,170,0); stroke-width: 2;" />
											<line x1="0" y1="7" x2="20" y2="14" style="stroke: rgb(255,170,0); stroke-width: 2;" />
										</svg>
										<div class="rpcEvent" style="float: right;">
											<xsl:value-of select="object"/>
										</div>
										<div class="rpcEvent">
											Result:<xsl:value-of select="description"/>
										</div>
										<div class="rpcEvent">
											<xsl:value-of select="detail"/>
										</div>
									</div>
								</div>
							</xsl:when>

							<xsl:when test="@type = 'CrpcError'">
								<div class="event" style="height: 50px;">
									<div class="objMid" style="left: 0px;"/>
									<div class="objMid" style="right: 0px;"/>
									<div class="crpcErrorEvent">
										<span class="rpcTooltip">
											<xsl:value-of select="original"/>
										</span>
										<xsl:value-of select="description"/>
									</div>
								</div>
							</xsl:when>

							<xsl:when test="@type = 'Exception'">
								<div class="event">
									<div class="exceptionEvent">
										<xsl:value-of select="description"/>
										<xsl:value-of select="original"/>
									</div>
								</div>
							</xsl:when>

							<xsl:otherwise>
								<div class="event">
									<div class="undefinedEvent">
										<xsl:value-of select="original"/>
									</div>
								</div>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:for-each>

					<div class="event">
						<div class="objBottom" style="float: left;"/>
						<div class="objBottom" style="float: right;"/>
						<div class="solidLines" style="clear: both;"/>
					</div>

				</div>

				<footer>
					<p>Copyright &#169; 2017 Ultamation Limited</p>
				</footer>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
