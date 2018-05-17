/**********************************************************\

  Auto-generated axdonetwrapperAPI.cpp

\**********************************************************/

#include "JSObject.h"
#include "variant_list.h"
#include "DOM/Document.h"
#include "axdonetwrapperAPI.h"

///////////////////////////////////////////////////////////////////////////////
/// @fn axdonetwrapperAPI::axdonetwrapperAPI(const axdonetwrapperPtr& plugin, const FB::BrowserHostPtr host)
///
/// @brief  Constructor for your JSAPI object.  You should register your methods, properties, and events
///         that should be accessible to Javascript from here.
///
/// @see FB::JSAPIAuto::registerMethod
/// @see FB::JSAPIAuto::registerProperty
/// @see FB::JSAPIAuto::registerEvent
///////////////////////////////////////////////////////////////////////////////
axdonetwrapperAPI::axdonetwrapperAPI(const axdonetwrapperPtr& plugin, const FB::BrowserHostPtr& host) : m_plugin(plugin), m_host(host)
{
    registerMethod("LoadPlugin",      make_method(this, &axdonetwrapperAPI::LoadPlugin));
    registerMethod("Initialize", make_method(this, &axdonetwrapperAPI::Initialize));

	registerMethod("DeInitialize",      make_method(this, &axdonetwrapperAPI::DeInitialize));
	registerMethod("CurrentURL", make_method(this, &axdonetwrapperAPI::CurrentURL));

	registerMethod("Excute",      make_method(this, &axdonetwrapperAPI::Excute));

	registerMethod("Get",      make_method(this, &axdonetwrapperAPI::Get));
	registerMethod("Set",      make_method(this, &axdonetwrapperAPI::Set));


    // Read-write property
    registerProperty("Host",
                     make_property(this,
                        &axdonetwrapperAPI::get_Host,
                        &axdonetwrapperAPI::set_Host));

	registerProperty("Id",
		make_property(this,
		&axdonetwrapperAPI::get_Id,
		&axdonetwrapperAPI::set_Id));

	registerProperty("UserData",
		make_property(this,
		&axdonetwrapperAPI::get_UserData,
		&axdonetwrapperAPI::set_UserData));


}

///////////////////////////////////////////////////////////////////////////////
/// @fn axdonetwrapperAPI::~axdonetwrapperAPI()
///
/// @brief  Destructor.  Remember that this object will not be released until
///         the browser is done with it; this will almost definitely be after
///         the plugin is released.
///////////////////////////////////////////////////////////////////////////////
axdonetwrapperAPI::~axdonetwrapperAPI()
{
}

///////////////////////////////////////////////////////////////////////////////
/// @fn axdonetwrapperPtr axdonetwrapperAPI::getPlugin()
///
/// @brief  Gets a reference to the plugin that was passed in when the object
///         was created.  If the plugin has already been released then this
///         will throw a FB::script_error that will be translated into a
///         javascript exception in the page.
///////////////////////////////////////////////////////////////////////////////
axdonetwrapperPtr axdonetwrapperAPI::getPlugin()
{
    axdonetwrapperPtr plugin(m_plugin.lock());
    if (!plugin) {
        throw FB::script_error("The plugin is invalid");
    }
    return plugin;
}


// Read/Write property testString
std::string axdonetwrapperAPI::get_Host()
{
    return getPlugin()->get_Host();
}
void axdonetwrapperAPI::set_Host(const std::string& val)
{
   getPlugin()->set_Host(val);
}

// Read/Write property testString
std::string axdonetwrapperAPI::get_Id()
{
	return getPlugin()->get_Id();
}
void axdonetwrapperAPI::set_Id(const std::string& val)
{
	getPlugin()->set_Id(val);
}

std::string axdonetwrapperAPI::get_UserData()
{
	return getPlugin()->get_UserData();
}

void axdonetwrapperAPI::set_UserData(const std::string& val)
{
	getPlugin()->set_UserData(val);
}

std::string axdonetwrapperAPI::LoadPlugin(const std::string &pluginName)
{
	return getPlugin()->LoadPlugin(pluginName);
}

std::string axdonetwrapperAPI::Initialize(const std::string& className)
{
	return getPlugin()->Initialize(className);
}

std::string axdonetwrapperAPI::DeInitialize()
{
	return getPlugin()->DeInitialize();
}

std::string axdonetwrapperAPI::CurrentURL(const std::string &url)
{
	return getPlugin()->CurrentURL(url);
}

std::string axdonetwrapperAPI::Excute(const std::string &args)
{
	return getPlugin()->Excute(args);
}

std::string axdonetwrapperAPI::Get(const std::string &propName)
{
	return getPlugin()->Get(propName);
}

std::string axdonetwrapperAPI::Set(const std::string &propName,const FB::variant& val)
{
	CComVariant   va;
	FBLOG_INFO("axdonetwrapperAPI",val.get_type().name());
	if(val.is_of_type<std::string>())
	{
		std::wstring wstr = val.convert_cast<std::wstring>();
		CComBSTR bStr(wstr.c_str());
		va=bStr;
	}
	if(val.is_of_type<double>()||val.is_of_type<long>()||val.is_of_type<int>())
	{
		va=val.convert_cast<long>();
	}
	if(val.is_of_type<bool>())
	{
		va=val.convert_cast<bool>();
	}
	std::string tmp= getPlugin()->Set(propName,va);
	VariantClear(&va);
	return tmp;
}
