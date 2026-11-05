<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="yes" encoding="UTF-8"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>List of dormitory's residents</title>
				<style>
					table { border-collapse: collapse; width: 100%; font-family: Times New Roman, sans-serif; }
					th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
					th { background-color: #2b0b3f; color: white; }
					tr:nth-child(even) { background-color: #f2f2f2; }
				</style>
			</head>
			<body>
				<h2>Information about residents' settlements</h2>
				<table>
					<tr>
						<th>Full name</th>
						<th>Faculty</th>
						<th>Department</th>
						<th>Course</th>
						<th>Room</th>
						<th>Dates of stay</th>
					</tr>
					<xsl:for-each select="Dormitory/Student">
						<tr>
							<td>
								<xsl:value-of select="FullName"/>
							</td>
							<td>
								<xsl:value-of select="@faculty"/>
							</td>
							<td>
								<xsl:value-of select="Department"/>
							</td>
							<td>
								<xsl:value-of select="@course"/>
							</td>
							<td>
								<xsl:value-of select="Residence/Room/@number"/>
							</td>
							<td>
								from <xsl:value-of select="Residence/Room/@checkIn"/>
								to <xsl:value-of select="Residence/Room/@checkOut"/>
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>