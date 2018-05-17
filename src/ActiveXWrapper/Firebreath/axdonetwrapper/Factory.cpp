/**********************************************************\ 
 
 Auto-generated Factory.cpp
 
 This file contains the auto-generated factory methods 
 for the axdonetwrapper project
 
\**********************************************************/

#include "FactoryBase.h"
#include "axdonetwrapper.h"
#include <boost/make_shared.hpp>
#include <direct.h>
#include <time.h>


class PluginFactory : public FB::FactoryBase
{
public:
    ///////////////////////////////////////////////////////////////////////////////
    /// @fn FB::PluginCorePtr createPlugin(const std::string& mimetype)
    ///
    /// @brief  Creates a plugin object matching the provided mimetype
    ///         If mimetype is empty, returns the default plugin
    ///////////////////////////////////////////////////////////////////////////////
    FB::PluginCorePtr createPlugin(const std::string& mimetype)
    {
        return boost::make_shared<axdonetwrapper>();
    }
    
    ///////////////////////////////////////////////////////////////////////////////
    /// @see FB::FactoryBase::globalPluginInitialize
    ///////////////////////////////////////////////////////////////////////////////
    void globalPluginInitialize()
    {
        axdonetwrapper::StaticInitialize();
    }
    
    ///////////////////////////////////////////////////////////////////////////////
    /// @see FB::FactoryBase::globalPluginDeinitialize
    ///////////////////////////////////////////////////////////////////////////////
    void globalPluginDeinitialize()
    {
        axdonetwrapper::StaticDeinitialize();
    }

	void getLoggingMethods( FB::Log::LogMethodList& outMethods )
	{
		// The next line will enable logging to the console (think: printf).
		outMethods.push_back(std::make_pair(FB::Log::LogMethod_Console, std::string()));

		std::string aTempFileName = "fblogs";

		DWORD nBufferLength = MAX_PATH;
		LPTSTR lpBuffer = (new TCHAR[nBufferLength]);
		DWORD tempPath = GetTempPath(nBufferLength, lpBuffer);

		char* localTempPathArray = new char[nBufferLength];
		for (int i = 0; i < nBufferLength; i++) {
			localTempPathArray[i] = (char)lpBuffer[i];
		}
		std::string localTempPath(localTempPathArray);
		localTempPath = localTempPath +"/"+ aTempFileName;

		_mkdir(localTempPath.c_str());

		time_t rawtime;
		struct tm * timeinfo;
		char buffer [128];
		time (&rawtime);


		timeinfo = localtime (&rawtime);
		strftime (buffer,sizeof(buffer),"%Y-%m-%d.log",timeinfo);
		std::string logfileName(buffer);

		outMethods.push_back(std::make_pair(FB::Log::LogMethod_File, localTempPath+"/"+logfileName));
	}
};

///////////////////////////////////////////////////////////////////////////////
/// @fn getFactoryInstance()
///
/// @brief  Returns the factory instance for this plugin module
///////////////////////////////////////////////////////////////////////////////
FB::FactoryBasePtr getFactoryInstance()
{
    static boost::shared_ptr<PluginFactory> factory = boost::make_shared<PluginFactory>();
    return factory;
}

