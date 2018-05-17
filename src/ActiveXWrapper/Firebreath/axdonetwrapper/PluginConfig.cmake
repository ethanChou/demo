#/**********************************************************\ 
#
# Auto-Generated Plugin Configuration file
# for axdonetwrapper
#
#\**********************************************************/

set(PLUGIN_NAME "axdonetwrapper")
set(PLUGIN_PREFIX "AXD")
set(COMPANY_NAME "richmars")

# ActiveX constants:
set(FBTYPELIB_NAME axdonetwrapperLib)
set(FBTYPELIB_DESC "axdonetwrapper 1.0 Type Library")
set(IFBControl_DESC "axdonetwrapper Control Interface")
set(FBControl_DESC "axdonetwrapper Control Class")
set(IFBComJavascriptObject_DESC "axdonetwrapper IComJavascriptObject Interface")
set(FBComJavascriptObject_DESC "axdonetwrapper ComJavascriptObject Class")
set(IFBComEventSource_DESC "axdonetwrapper IFBComEventSource Interface")
set(AXVERSION_NUM "1")

# NOTE: THESE GUIDS *MUST* BE UNIQUE TO YOUR PLUGIN/ACTIVEX CONTROL!  YES, ALL OF THEM!
set(FBTYPELIB_GUID 70fcc90b-71d5-5b21-947c-07b2d4e0d4b8)
set(IFBControl_GUID ce3cbe1e-1257-502c-8269-69711941d867)
set(FBControl_GUID 5cabb28d-d086-5249-8234-dfd08d6fb588)
set(IFBComJavascriptObject_GUID 60f6bebb-0314-518d-8f03-ecd72cbb83c8)
set(FBComJavascriptObject_GUID 8d52bf12-07bd-5bae-ab60-9ba36ea9715e)
set(IFBComEventSource_GUID ec1927e0-96e8-5a89-808d-9082b023926d)

# these are the pieces that are relevant to using it from Javascript
set(ACTIVEX_PROGID "richmars.axdonetwrapper")
set(MOZILLA_PLUGINID "richmars.com/axdonetwrapper")

# strings
set(FBSTRING_CompanyName "richmars.com")
set(FBSTRING_FileDescription "richmars.com")
set(FBSTRING_PLUGIN_VERSION "1.0.0.0")
set(FBSTRING_LegalCopyright "Copyright 2018 richmars.com")
set(FBSTRING_PluginFileName "np${PLUGIN_NAME}.dll")
set(FBSTRING_ProductName "axdonetwrapper")
set(FBSTRING_FileExtents "")
set(FBSTRING_PluginName "axdonetwrapper")
set(FBSTRING_MIMEType "application/x-axdonetwrapper")

# Uncomment this next line if you're not planning on your plugin doing
# any drawing:

#set (FB_GUI_DISABLED 1)

# Mac plugin settings. If your plugin does not draw, set these all to 0
set(FBMAC_USE_QUICKDRAW 0)
set(FBMAC_USE_CARBON 1)
set(FBMAC_USE_COCOA 1)
set(FBMAC_USE_COREGRAPHICS 1)
set(FBMAC_USE_COREANIMATION 0)
set(FBMAC_USE_INVALIDATINGCOREANIMATION 0)

# If you want to register per-machine on Windows, uncomment this line
#set (FB_ATLREG_MACHINEWIDE 1)

add_firebreath_library(log4cplus)
