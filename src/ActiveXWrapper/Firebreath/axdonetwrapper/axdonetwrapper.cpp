/**********************************************************\

  Auto-generated axdonetwrapper.cpp

  This file contains the auto-generated main plugin object
  implementation for the axdonetwrapper project

\**********************************************************/

#include "axdonetwrapperAPI.h"

#include "axdonetwrapper.h"

#include "DOM/Window.h"

//未发现axtivex控件
#define  RES_ERROR -128
//调用方法失败
#define  RES_FAIL -129

#define  RES_EMPTY ""
#define  RES_SUCCESS 0
#define  RES_EXCUTE_ECEPTION "excute method fail"
#define  RES_ACTIVEX_FAIL "activex initialize fail"


EXTERN_C IMAGE_DOS_HEADER __ImageBase;
#define HINST_THISCOMPONENT ((HINSTANCE)&__ImageBase)


_ATL_FUNC_INFO efiCustumEvent = {CC_STDCALL, VT_EMPTY, 3, {VT_BSTR,VT_BSTR,VT_BSTR}};

/**********************************************************\

  implementation for the ActiveX control container class

\**********************************************************/

void __stdcall axdonetwrapperWin::onCustumEventFired(CComBSTR id,CComBSTR eventName, CComBSTR data)
{
	try {
		std::string str_id=FB::wstring_to_utf8(std::wstring(id.m_str, id.Length()));
		std::string str_evt=FB::wstring_to_utf8(std::wstring(eventName.m_str, eventName.Length()));
		std::string str_dt=FB::wstring_to_utf8(std::wstring(data.m_str, data.Length()));

		FBLOG_INFO("axdonetwrapperWin","fire_CustumEventFired begin");

		m_pPlugin->getJSAPI()->fire_CustumEventFired(str_id,str_evt,str_dt);

		FBLOG_INFO("axdonetwrapperWin","fire_CustumEventFired end");
	}
	catch(...)
	{
		FBLOG_INFO("axdonetwrapperWin","fire_CustumEventFired error");
	}
}

///////////////////////////////////////////////////////////////////////////////
/// @fn axdonetwrapper::StaticInitialize()
///
/// @brief  Called from PluginFactory::globalPluginInitialize()
///
/// @see FB::FactoryBase::globalPluginInitialize
///////////////////////////////////////////////////////////////////////////////
void axdonetwrapper::StaticInitialize()
{
    // Place one-time initialization stuff here; As of FireBreath 1.4 this should only
    // be called once per process
}

///////////////////////////////////////////////////////////////////////////////
/// @fn axdonetwrapper::StaticInitialize()
///
/// @brief  Called from PluginFactory::globalPluginDeinitialize()
///
/// @see FB::FactoryBase::globalPluginDeinitialize
///////////////////////////////////////////////////////////////////////////////
void axdonetwrapper::StaticDeinitialize()
{
    // Place one-time deinitialization stuff here. As of FireBreath 1.4 this should
    // always be called just before the plugin library is unloaded
}

///////////////////////////////////////////////////////////////////////////////
/// @brief  axdonetwrapper constructor.  Note that your API is not available
///         at this point, nor the window.  For best results wait to use
///         the JSAPI object until the onPluginReady method is called
///////////////////////////////////////////////////////////////////////////////
axdonetwrapper::axdonetwrapper():m_axwin(this)
{
	m_WindowAttached = false;
	m_PlugInReady = false;
	FBLOG_INFO("axdonetwrapper","constructor");
}

///////////////////////////////////////////////////////////////////////////////
/// @brief  axdonetwrapper destructor.
///////////////////////////////////////////////////////////////////////////////
axdonetwrapper::~axdonetwrapper()
{
    // This is optional, but if you reset m_api (the shared_ptr to your JSAPI
    // root object) and tell the host to free the retained JSAPI objects then
    // unless you are holding another shared_ptr reference to your JSAPI object
    // they will be released here.
    releaseRootJSAPI();
    m_host->freeRetainedObjects();
}

void axdonetwrapper::onPluginReady()
{
	FBLOG_INFO("axdonetwrapper","onPluginReady");
	// When this is called, the BrowserHost is attached, the JSAPI object is
	// created, and we are ready to interact with the page and such.  The
	// PluginWindow may or may not have already fire the AttachedEvent at
	// this point.
	if(m_PlugInReady == false)
	{
		m_PlugInReady = true;
		if(m_WindowAttached)
			CreateAxWindow();
	}
}

bool axdonetwrapper::setReady()
{
	FBLOG_INFO("axdonetwrapper","setReady");
	// The call to getRootJSAPI() was removed from PluginCore::setReady() between version
	// 1.5 and 1.6. Without this call, the plugin JSAPI object may not get created unless
	// the html <object> tag includes an "onload" param. This plugin relies on the pointer
	// to the JSAPI object being valid in order to fire events.

	// Ensure that the JSAPI object has been created, in case the browser hasn't requested it yet.
	getRootJSAPI(); 
	return PluginCore::setReady();
}


///////////////////////////////////////////////////////////////////////////////
/// @brief  Creates an instance of the JSAPI object that provides your main
///         Javascript interface.
///
/// Note that m_host is your BrowserHost and shared_ptr returns a
/// FB::PluginCorePtr, which can be used to provide a
/// boost::weak_ptr<axdonetwrapper> for your JSAPI class.
///
/// Be very careful where you hold a shared_ptr to your plugin class from,
/// as it could prevent your plugin class from getting destroyed properly.
///////////////////////////////////////////////////////////////////////////////
FB::JSAPIPtr axdonetwrapper::createJSAPI()
{
	FBLOG_INFO("axdonetwrapper","createJSAPI");
	// m_host is the BrowserHost
	m_axwrapperapi = boost::make_shared<axdonetwrapperAPI>(FB::ptr_cast<axdonetwrapper>(shared_from_this()), m_host);
	return m_axwrapperapi;
}


bool axdonetwrapper::onWindowsMessage(FB::WindowsEvent *evt, FB::PluginWindowWin *piw)
{
	bool rc = false;
	if(!m_axwin)
		return false;
	switch(evt->uMsg)
	{
	case WM_SIZE:
		// resize the ActiveX container window
		m_axwin.MoveWindow(0, 0, LOWORD(evt->lParam), HIWORD(evt->lParam));
		evt->lRes = 0;
		rc = true;
		break;
	case WM_MOUSEACTIVATE:
		// activate on mouse click
		evt->lRes = MA_ACTIVATE;
		rc = true;
		break;
	case WM_SETFOCUS:
		// forward focus to the ActiveX control container window
		m_axwin.SetFocus();
		evt->lRes = 0;
		rc = true;
		break;
	}
	return rc;
}

bool axdonetwrapper::onWindowAttached(FB::AttachedEvent *evt, FB::PluginWindow *)
{
	if(m_WindowAttached == false)
	{
		m_WindowAttached = true;
		if(m_PlugInReady)
			CreateAxWindow();
	}
    return false;
}

bool axdonetwrapper::onWindowDetached(FB::DetachedEvent *evt, FB::PluginWindow *)
{
     DestroyAxWindow();
    return false;
}

bool axdonetwrapper::CreateAxWindow()
{
	FBLOG_INFO("axdonetwrapper","CreateAxWindow");
	try {

		/* Now that we have the plugin window, create the ActiveX container
		   window as a child of the plugin, then create the ActiveX control
			as a child of the container.
		*/
		FB::PluginWindowWin* pwnd = static_cast<FB::PluginWindowWin*>(GetWindow());
		if(!IsWindow(pwnd->getHWND())){
			FBLOG_INFO("axdonetwrapper","IsWindow false");
			return false;
		}
		if(pwnd != NULL)
		{
			HWND hWnd = pwnd->getHWND();
			if(hWnd)
			{				
				// Logger logger = Logger::getInstance("main");  
				// Create the ActiveX control container
				RECT rc;
				::GetClientRect(hWnd, &rc);
				m_axwin.Create(hWnd, &rc, 0, WS_VISIBLE|WS_CHILD);

				// Create an instance of the ActiveX control in the container. If the ActiveX
				// control requires a license key, change CreateControlEx to CreateControlLicEx
				// and add one more parameter - CComBSTR(AXCTLLICKEY) - to the argument list.
				CComPtr<IUnknown> spControl;
				HRESULT hr = m_axwin.CreateControlEx(AXCTLPROGID, NULL, NULL, &spControl, GUID_NULL, NULL);
				if(SUCCEEDED(hr) && (spControl != NULL))
				{
					FBLOG_INFO("axdonetwrapper","createcontrolex success");
					// Get the control's default interface
					spControl.QueryInterface(&m_spaxctl);
					if(m_spaxctl)
					{
						FBLOG_INFO("axdonetwrapper","QueryInterface success");

						// Connect the event sink
						hr = m_axwin.DispEventAdvise((IUnknown*)m_spaxctl);

					    FBLOG_INFO("axdonetwrapper","DispEventAdvise success");

						try
						{
							std::string location = getHost()->getDOMWindow()->getLocation();
							CurrentURL(location);
						}
						catch (...)
						{
							FBLOG_INFO("axdonetwrapper","location error");
						}

						std::string pluginname;
						try {
							pluginname = m_params["pluginname"].convert_cast<std::string>();
							LoadPlugin(pluginname);
							
							FBLOG_INFO("axdonetwrapper","LoadPlugin pluginname ok");
						} catch(...) {
							FBLOG_INFO("axdonetwrapper","LoadPlugin pluginname error");
						}


						std::string classname;
						try {
							classname = m_params["classname"].convert_cast<std::string>();
							Initialize(classname);
							
							FBLOG_INFO("axdonetwrapper","Initialize classname ok");
						} catch(...) {
							FBLOG_INFO("axdonetwrapper","Initialize classname error");
						} 

						return true;
					}
					else{
						FBLOG_INFO("axdonetwrapper","QueryInterface error");
					}
				}
				else{

					HWND hButton = CreateWindow(L"STATIC", L"请调用ActiveX失败,请重新注册", WS_VISIBLE | WS_CHILD | BS_PUSHBUTTON,  
						0, 0, rc.right-rc.left, rc.bottom-rc.top, hWnd, NULL, HINST_THISCOMPONENT, NULL);  

					FBLOG_INFO("axdonetwrapper","CreateControlEx error");
				}
			}
		}
		else{
			FBLOG_INFO("axdonetwrapper","pwnd is null");
		}
	} catch(...) {
		FBLOG_INFO("axdonetwrapper","createaxwindow error");
	}
	return false;
}

void axdonetwrapper::DestroyAxWindow()
{
	FBLOG_INFO("axdonetwrapper","DestroyAxWindow");
	if(m_spaxctl)
	{
		CComBSTR bstr;
		HRESULT hr = m_spaxctl->DeInitialize(&bstr);

		// Disconnect the event sink
		m_axwin.DispEventUnadvise((IUnknown*)m_spaxctl);		
		// Kill reference to the ActiveX control - when the plugin
		// window is destroyed, the container & control will be
		// automatically destroyed.
		m_spaxctl = NULL;
		m_WindowAttached=false;
	}
}


 std::string axdonetwrapper::LoadPlugin(const std::string& pluginName){
	if(m_spaxctl)
	{
		FBLOG_INFO("axdonetwrapper","LoadPlugin args:"+pluginName);

		try {
			CComBSTR bstr;
			HRESULT hr = m_spaxctl->LoadPlugin(CComBSTR(FB::utf8_to_wstring(pluginName).c_str()),&bstr);
			if(SUCCEEDED(hr))
			{
				return FB::wstring_to_utf8(std::wstring(bstr.m_str, bstr.Length()));
			}
			else{
				return GetResponse(RES_FAIL,RES_EXCUTE_ECEPTION,RES_EMPTY);
			}
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","LoadPlugin error ");
		}
	}
	return GetResponse(RES_ERROR,RES_ACTIVEX_FAIL,RES_EMPTY);
}

std::string axdonetwrapper::Initialize(const std::string& className){
	if(m_spaxctl)
	{
		//std::stringstream ss;
		//std::string str;
		//ss<<modelId;
		//ss>>str;
		
		FBLOG_INFO("axdonetwrapper","Initialize args:"+className);
	
		try {
			CComBSTR bstr;
			HRESULT hr = m_spaxctl->Initialize(CComBSTR(FB::utf8_to_wstring(className).c_str()),&bstr);
			if(SUCCEEDED(hr))
			{
				return FB::wstring_to_utf8(std::wstring(bstr.m_str, bstr.Length()));
			}
			else{
				return GetResponse(RES_FAIL,RES_EXCUTE_ECEPTION,RES_EMPTY);
			}
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","Initialize error ");
		}
	}
	return GetResponse(RES_ERROR,RES_ACTIVEX_FAIL,RES_EMPTY);
}

std::string axdonetwrapper::DeInitialize(){
	if(m_spaxctl)
	{
		FBLOG_INFO("axdonetwrapper","DeInitialize");
		try {
			CComBSTR bstr;
			HRESULT hr = m_spaxctl->DeInitialize(&bstr);
			if(SUCCEEDED(hr))
			{
				return FB::wstring_to_utf8(std::wstring(bstr.m_str, bstr.Length()));
			}
			else{
				return GetResponse(RES_FAIL,RES_EXCUTE_ECEPTION,RES_EMPTY);
			}
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","DeInitialize");
		}
	}
	return GetResponse(RES_ERROR,RES_ACTIVEX_FAIL,RES_EMPTY);
}

std::string axdonetwrapper::get_Host()
{
	if(m_spaxctl)
	{
		try {
			CComBSTR bstr;
			HRESULT hr = m_spaxctl->get_Host(&bstr);
			if(SUCCEEDED(hr))
			{
				return FB::wstring_to_utf8(std::wstring(bstr.m_str, bstr.Length()));
			}
			else{
				return GetResponse(RES_FAIL,RES_EXCUTE_ECEPTION,RES_EMPTY);
			}
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","get_Host error ");
		}
	}
	return GetResponse(RES_ERROR,RES_ACTIVEX_FAIL,RES_EMPTY);
}

void axdonetwrapper::set_Host(const std::string& val)
{
	if(m_spaxctl)
	{
		FBLOG_INFO("axdonetwrapper","set_Host args:"+val);
		try {
			HRESULT hr = m_spaxctl->put_Host(CComBSTR(FB::utf8_to_wstring(val).c_str()));
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","set_Host error ");
		}
	}
}

std::string axdonetwrapper::get_Id()
{
	if(m_spaxctl)
	{
		try {
			CComBSTR bstr;
			HRESULT hr = m_spaxctl->get_Id(&bstr);
			if(SUCCEEDED(hr))
			{
				return FB::wstring_to_utf8(std::wstring(bstr.m_str, bstr.Length()));
			}
			else{
				return GetResponse(RES_FAIL,RES_EXCUTE_ECEPTION,RES_EMPTY);
			}
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","get_Host error ");
		}
	}
	return GetResponse(RES_ERROR,RES_ACTIVEX_FAIL,RES_EMPTY);
}

void axdonetwrapper::set_Id(const std::string& val)
{
	if(m_spaxctl)
	{
		FBLOG_INFO("axdonetwrapper","set_Host args:"+val);
		try {
			HRESULT hr = m_spaxctl->put_Id(CComBSTR(FB::utf8_to_wstring(val).c_str()));
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","set_Host error ");
		}
	}
}

std::string axdonetwrapper::get_UserData()
{
	if(m_spaxctl)
	{
		try {
			CComBSTR bstr;
			HRESULT hr = m_spaxctl->get_UserData(&bstr);
			if(SUCCEEDED(hr))
			{
				return FB::wstring_to_utf8(std::wstring(bstr.m_str, bstr.Length()));
			}
			else{
				return GetResponse(RES_FAIL,RES_EXCUTE_ECEPTION,RES_EMPTY);
			}
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","get_UserData error ");
		}
	}
	return GetResponse(RES_ERROR,RES_ACTIVEX_FAIL,RES_EMPTY);
}

void axdonetwrapper::set_UserData(const std::string& val)
{
	if(m_spaxctl)
	{
		FBLOG_INFO("axdonetwrapper","set_UserData");
		//ActiveX::makeComVariant(m_host,)
		try {
			HRESULT hr = m_spaxctl->put_UserData(CComBSTR(FB::utf8_to_wstring(val).c_str()));
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","set_Hset_UserDataost error ");
		}
	}
}

std::string axdonetwrapper::CurrentURL(const std::string& location){
	if(m_spaxctl)
	{
		FBLOG_INFO("axdonetwrapper","CurrentURL args:"+location);
		try {
			CComBSTR bstr;
			HRESULT hr = m_spaxctl->CurrentURL(CComBSTR(FB::utf8_to_wstring(location).c_str()),&bstr);
			if(SUCCEEDED(hr))
			{
				return FB::wstring_to_utf8(std::wstring(bstr.m_str, bstr.Length()));
			}
			else{
				return GetResponse(RES_FAIL,RES_EXCUTE_ECEPTION,RES_EMPTY);
			}
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","CurrentURL error ");
		}
	}
	return GetResponse(RES_ERROR,RES_ACTIVEX_FAIL,RES_EMPTY);
}

std::string axdonetwrapper::Excute(const std::string& args){

	if(m_spaxctl)
	{
		FBLOG_INFO("axdonetwrapper","Excute ");
		try {
			CComBSTR bstr;
			HRESULT hr = m_spaxctl->Excute(CComBSTR(FB::utf8_to_wstring(args).c_str()),&bstr);
			if(SUCCEEDED(hr))
			{
				return FB::wstring_to_utf8(std::wstring(bstr.m_str, bstr.Length()));
			}
			else{
				return GetResponse(RES_FAIL,RES_EXCUTE_ECEPTION,RES_EMPTY);
			}
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","Excute error ");
		}
	}
return GetResponse(RES_ERROR,RES_ACTIVEX_FAIL,RES_EMPTY);
}

std::string axdonetwrapper::Get(const std::string& propName){

	if(m_spaxctl)
	{
		FBLOG_INFO("axdonetwrapper","Get ");
		try {
			CComBSTR bstr;
			HRESULT hr = m_spaxctl->Get(CComBSTR(FB::utf8_to_wstring(propName).c_str()),&bstr);
			if(SUCCEEDED(hr))
			{
				return FB::wstring_to_utf8(std::wstring(bstr.m_str, bstr.Length()));
			}
			else{
				return GetResponse(RES_FAIL,RES_EXCUTE_ECEPTION,RES_EMPTY);
			}
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","Get error ");
		}
	}
	return GetResponse(RES_ERROR,RES_ACTIVEX_FAIL,RES_EMPTY);
}

std::string axdonetwrapper::Set(const std::string& propName,const CComVariant& val){

	if(m_spaxctl)
	{
		FBLOG_INFO("axdonetwrapper","Set ");
		try {
			CComBSTR bstr;
			HRESULT hr = m_spaxctl->Set(CComBSTR(FB::utf8_to_wstring(propName).c_str()),val,&bstr);
			if(SUCCEEDED(hr))
			{
				return FB::wstring_to_utf8(std::wstring(bstr.m_str, bstr.Length()));
			}
			else{
				return GetResponse(RES_FAIL,RES_EXCUTE_ECEPTION,RES_EMPTY);
			}
		}
		catch(...) {
			FBLOG_INFO("axdonetwrapper","Set error ");
		}
	}
	return GetResponse(RES_ERROR,RES_ACTIVEX_FAIL,RES_EMPTY);
}

