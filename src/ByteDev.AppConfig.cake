string AppConfigGetAppSetting(string appConfigPath, string key)
{
	var xPath = @"/configuration/appSettings/add[@key='" + key + "']/@value";

	return XmlPeek(appConfigPath, xPath);
}

void AppConfigUpdateAppSetting(string appConfigPath, string key, string newValue)
{
	var xPath = @"/configuration/appSettings/add[@key='" + key + "']/@value";

	XmlPoke(appConfigPath, xPath, newValue);
}

