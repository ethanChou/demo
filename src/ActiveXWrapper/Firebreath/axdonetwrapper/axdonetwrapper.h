/**********************************************************\

  Auto-generated axdonetwrapper.h

  This file contains the auto-generated main plugin object
  implementation for the axdonetwrapper project

\**********************************************************/
#ifndef H_axdonetwrapperPLUGIN
#define H_axdonetwrapperPLUGIN

#include "PluginWindow.h"
#include "Win\PluginWindowWin.h"
#include "PluginEvents/MouseEvents.h"
#include "PluginEvents/AttachedEvent.h"
#include "PluginEvents/WindowsEvent.h"
#include "PluginCore.h"
#include <atlwin.h>

class axdonetwrapper;
class axdonetwrapperAPI;


// Import the ActiveX control's typelib so we can easily call methods, etc.
// on the ActiveX control.
#import "..\..\..\bin\ActiveXHost.tlb" named_guids,no_namespace, raw_interfaces_only

// Define the ProgID for the ActiveX control.
#define AXCTLPROGID L"ActiveXHost.HostContainer"

// Define the ActiveX control's default & event interfaces. You might
// want to use a type library view like OleView.exe to identify interface
// names. Or you can find them in the registry.
#define AXCTLDEFINTF IHostContainer
#define AXCTLEVTINTF IHostContainerEvent



// Event function info structures are defined in axWrapper.cpp
extern _ATL_FUNC_INFO efiCustumEvent;


/**********************************************************\
 Define the ActiveX control container class using ATL's CAxWindow
 template with event support from ATL's IDispEventSimpleImpl
 tempalte.
 Note: if the ActiveX control that you are wrapping requires a
 license key, use CAxWindow2 instead of CAxWindow.
 \**********************************************************/
class axdonetwrapperWin : public CAxWindow, public IDispEventSimpleImpl<1, axdonetwrapperWin, &__uuidof(AXCTLEVTINTF)>
{
public:

	axdonetwrapperWin(axdonetwrapper* pPlugin) : m_pPlugin(pPlugin) {}
	virtual ~axdonetwrapperWin() {}

	// Declare the events from the ActiveX control that we want to catch
	// in our FB plugin. See ATL documentation for IDispEventSimpleImpl for
	// details.
	BEGIN_SINK_MAP(axdonetwrapperWin)
		SINK_ENTRY_INFO(1, __uuidof(AXCTLEVTINTF), 0x0001, onCustumEventFired, &efiCustumEvent)
	END_SINK_MAP()

	// Declare the ActiveX control event handler functions

	void __stdcall onCustumEventFired(CComBSTR id,CComBSTR eventName,CComBSTR data);

	// Back pointer to containing plugin object (this is not using boost
	// shared_ptr because this object's lifetime is controlled excusively
	// by the plugin object's lifetime).
	axdonetwrapper* m_pPlugin; 

};


FB_FORWARD_PTR(axdonetwrapper)
class axdonetwrapper : public FB::PluginCore
{
public:
    static void StaticInitialize();
    static void StaticDeinitialize();

public:
    axdonetwrapper();
    virtual ~axdonetwrapper();

public:
    void onPluginReady();
    //void shutdown();
    virtual FB::JSAPIPtr createJSAPI();
    // If you want your plugin to always be windowless, set this to true
    // If you want your plugin to be optionally windowless based on the
    // value of the "windowless" param tag, remove this method or return
    // FB::PluginCore::isWindowless()
    //virtual bool isWindowless() { return false; }
	virtual bool setReady();

    BEGIN_PLUGIN_EVENT_MAP()
		EVENTTYPE_CASE(FB::WindowsEvent, onWindowsMessage, FB::PluginWindowWin)
        EVENTTYPE_CASE(FB::AttachedEvent, onWindowAttached, FB::PluginWindow)
        EVENTTYPE_CASE(FB::DetachedEvent, onWindowDetached, FB::PluginWindow)
    END_PLUGIN_EVENT_MAP()

    /** BEGIN EVENTDEF -- DON'T CHANGE THIS LINE **/
	virtual bool onWindowsMessage(FB::WindowsEvent *evt, FB::PluginWindowWin *);
    virtual bool onWindowAttached(FB::AttachedEvent *evt, FB::PluginWindow *);
    virtual bool onWindowDetached(FB::DetachedEvent *evt, FB::PluginWindow *);
    /** END EVENTDEF -- DON'T CHANGE THIS LINE **/
public:

	std::string LoadPlugin(const std::string& pluginName);

	std::string Initialize(const std::string& className);

	std::string DeInitialize();

	std::string get_Host();

	//host: 172.16.62.59:8088
	void set_Host(const std::string& host);

	std::string get_Id();

	//host: 172.16.62.59:8088
	void set_Id(const std::string& host);

	std::string get_UserData();

	void set_UserData(const std::string& tag);

	std::string Get(const std::string& propName);

	std::string Set(const std::string& propName,const CComVariant& val);

	std::string CurrentURL(const std::string& location);

	std::string Excute(const std::string& args);
	// provide access to JSAPI functions
	typedef boost::shared_ptr<axdonetwrapperAPI> axdonetwrapperAPIPtr;
	axdonetwrapperAPIPtr getJSAPI() {return m_axwrapperapi;}

private:

	std::string GetResponse(const int code,const std::string &data,const std::string &msg){
		return format("{\"code\":%d,\"data\":\"%s\",\"msg\":\"%s\"}",code,data.data(),msg.data());
	}

	std::string format(const char *pszFmt, ...)
	{
		std::string str;
		va_list args;
		va_start(args, pszFmt);
		{
			int nLength = _vscprintf(pszFmt, args);
			nLength += 1;  
			//上面返回的长度是包含\0，这里加上
			std::vector<char> vectorChars(nLength);
			_vsnprintf(vectorChars.data(), nLength, pszFmt, args);
			str.assign(vectorChars.data());
		}
		va_end(args);
		return str;
	}

	FB::BrowserHostPtr getHost() {return m_host;}

	// ActiveX control initialization
	bool InitializeAxControl();
	bool CreateAxWindow();
	void DestroyAxWindow();

	// Flags to ensure we wait until both onPlugInReady & onWindowAttached
	// have been called before creating the CAxWindow.
	bool m_WindowAttached;
	bool m_PlugInReady;

	axdonetwrapperAPIPtr m_axwrapperapi; // easy access to JSAPI functions
	axdonetwrapperWin m_axwin; // ActiveX control container instance (CAxWindow)
	CComPtr<AXCTLDEFINTF> m_spaxctl; // ActiveX control instance
};


typedef boost::shared_ptr<axdonetwrapper> axdonetwrapperPtr;
typedef boost::weak_ptr<axdonetwrapper> axdonetwrapperWeakPtr;

#endif

