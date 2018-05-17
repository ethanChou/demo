/**********************************************************\

  Auto-generated axdonetwrapperAPI.h

\**********************************************************/

#include <string>
#include <sstream>
#include <boost/weak_ptr.hpp>
#include "JSAPIAuto.h"
#include "BrowserHost.h"
#include "axdonetwrapper.h"

#ifndef H_axdonetwrapperAPI
#define H_axdonetwrapperAPI

class axdonetwrapperAPI : public FB::JSAPIAuto
{
public:
    axdonetwrapperAPI(const axdonetwrapperPtr& plugin, const FB::BrowserHostPtr& host);
    virtual ~axdonetwrapperAPI();

    axdonetwrapperPtr getPlugin();

	std::string LoadPlugin(const std::string &pluginName);

	std::string Initialize(const std::string &className);

	std::string DeInitialize();

	std::string get_Host();

	//host: 172.16.62.59:8088
	void set_Host(const std::string& host);

	std::string get_Id();

	//host: 172.16.62.59:8088
	void set_Id(const std::string& host);

	std::string get_UserData();

	void set_UserData(const std::string& tag);

	std::string CurrentURL(const std::string& location);

	std::string Excute(const std::string& args);
    
	std::string Get(const std::string& propName);

	std::string Set(const std::string& propName,const FB::variant& val);

    // Event helpers
    FB_JSAPI_EVENT(CustumEventFired, 3, (const std::string&, const std::string&,const std::string&));

private:
    axdonetwrapperWeakPtr m_plugin;
    FB::BrowserHostPtr m_host;

};

#endif // H_axdonetwrapperAPI

