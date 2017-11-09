; �ýű�ʹ�� HM VNISEdit �ű��༭���򵼲���

; ��װ�����ʼ���峣��
!define PRODUCT_NAME "�����ҹ���ϵͳ"
!define PRODUCT_VERSION "2.0.0.0"
!define PRODUCT_PUBLISHER "homingfly, Inc."
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\VisitorManager.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

!define /date BUILTTIMESTAMP "%Y%m%d%H%M%S"

SetCompressor lzma

; ------ MUI �ִ����涨�� (1.67 �汾���ϼ���) ------
!include "MUI.nsh"

; MUI Ԥ���峣��
!define MUI_ABORTWARNING
!define MUI_ICON "logo.ico"
!define MUI_UNICON "uninst.ico"

!define MUI_WELCOMEFINISHPAGE_BITMAP "welcome.bmp"

; ��ӭҳ��
!insertmacro MUI_PAGE_WELCOME
; ��װĿ¼ѡ��ҳ��
!insertmacro MUI_PAGE_DIRECTORY
; ��װ����ҳ��
!insertmacro MUI_PAGE_INSTFILES
; ��װ���ҳ��
!define MUI_FINISHPAGE_RUN "$INSTDIR\VisitorManager.exe"
!insertmacro MUI_PAGE_FINISH

; ��װж�ع���ҳ��
!insertmacro MUI_UNPAGE_INSTFILES

; ��װ�����������������
!insertmacro MUI_LANGUAGE "SimpChinese"

; ��װԤ�ͷ��ļ�
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS
; ------ MUI �ִ����涨����� ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"

OutFile "VSMSetup_${PRODUCT_VERSION}_X86_Build${BUILTTIMESTAMP}.exe"

InstallDir "$PROGRAMFILES\VisitorManager"
InstallDirRegKey HKLM "${PRODUCT_UNINST_KEY}" "UninstallString"
ShowInstDetails show
ShowUnInstDetails show
BrandingText "�����ҹ���ϵͳ"

Section "MainSection" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer

  CreateDirectory "$SMPROGRAMS\�����ҹ���ϵͳ"
  CreateShortCut "$SMPROGRAMS\�����ҹ���ϵͳ\�����ҹ���ϵͳ.lnk" "$INSTDIR\VisitorManager.exe"
  CreateShortCut "$DESKTOP\�����ҹ���ϵͳ.lnk" "$INSTDIR\VisitorManager.exe"
  
  File "..\bin\Release\WPF.Extend.dll"
  File "..\bin\Release\WPF.Extend.Controls.dll"
  File "..\bin\Release\VisitorManager.ViewModel.dll"
  File "..\bin\Release\VisitorManager.exe.config"
  File "..\bin\Release\VisitorManager.exe"
  File "..\bin\Release\ThriftCommon.dll"
  File "..\bin\Release\Thrift.dll"
  File "..\bin\Release\System.Windows.Interactivity.xml"
  File "..\bin\Release\System.Windows.Interactivity.dll"
  File "..\bin\Release\System.Utility.dll"
  File "..\bin\Release\System.Data.SQLite.DLL"
  File "..\bin\Release\PresentationFramework.dll"
  File "..\bin\Release\PresentationFramework.Aero.dll"
  File "..\bin\Release\PresentationCore.dll"
  File "..\bin\Release\NPOI.xml"
  File "..\bin\Release\NPOI.OpenXmlFormats.dll"
  File "..\bin\Release\NPOI.OpenXml4Net.xml"
  File "..\bin\Release\NPOI.OpenXml4Net.dll"
  File "..\bin\Release\NPOI.OOXML.xml"
  File "..\bin\Release\NPOI.OOXML.dll"
  File "..\bin\Release\NPOI.dll"
  File "..\bin\Release\NLog.xml"
  File "..\bin\Release\NLog.dll"
  File "..\bin\Release\Newtonsoft.Json.dll"
  File "..\bin\Release\NetMQ.dll"
  File "..\bin\Release\MySql.Data.dll"
  File "..\bin\Release\Microsoft.Windows.Shell.dll"
  File "..\bin\Release\Microsoft.Expression.Interactions.xml"
  File "..\bin\Release\Microsoft.Expression.Interactions.dll"
  File "..\bin\Release\Microsoft.Expression.Effects.xml"
  File "..\bin\Release\Microsoft.Expression.Effects.dll"
  File "..\bin\Release\Microsoft.Expression.Drawing.xml"
  File "..\bin\Release\Microsoft.Expression.Drawing.dll"
  File "..\bin\Release\ICSharpCode.SharpZipLib.dll"
  File "..\bin\Release\ExcelReport.dll"
  File "..\bin\Release\Dapper.dll"
  File "..\bin\Release\Config.ini"
  File "..\bin\Release\CodeReason.Reports.dll"
  File "..\bin\Release\AsyncIO.dll"
  File "..\bin\Release\Apache.NMS.xml"
  File "..\bin\Release\Apache.NMS.dll"
  File "..\bin\Release\Apache.NMS.ActiveMQ.xml"
  File "..\bin\Release\Apache.NMS.ActiveMQ.dll"
  File "..\bin\Release\AForge.Video.dll"
  File "..\bin\Release\AForge.Video.DirectShow.dll"
  File "..\bin\Release\AForge.Controls.dll"
  
  SetOutPath "$INSTDIR\Doc"
  File /r /x *svn  "..\bin\Release\Doc\*"
  
  SetOutPath "$INSTDIR\Image"
  File /r /x *svn  "..\bin\Release\Image\*"
  
  SetOutPath "$INSTDIR\SDK"
  File /r /x *svn  "..\bin\Release\SDK\*"
  
  call InstallNetFramework4.5.2CHS
SectionEnd


Function InstallNetFramework4.5.2CHS
  ;��װ Net Framework 4.5.2 �������԰�
  SetDetailsPrint textonly
  DetailPrint "���ڰ�װ .NET Framework 4.5.2 CHS..."

  ClearErrors
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\2052" "Release"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\1033" "Release"
    IfErrors NotDetected
    ${If} $0 >= 379893
        DetailPrint "Microsoft .NET Framework 4.5.2 CHS is installed ($0)"
    ${OrIf} $1 >= 379893
   			DetailPrint "Microsoft .NET Framework 4.5.2 CHS is installed ($1)"
    ${Else}
    NotDetected:
        DetailPrint "Installing Microsoft .NET Framework 4.5.2 CHS"
        SetDetailsPrint listonly
        ExecWait '"$INSTDIR\Doc\1-1-NDP452-KB2901907-x86-x64-AllOS-ENU.exe" /norestart' $0
        ${If} $0 == 3010
        ${OrIf} $0 == 1641
            DetailPrint "Microsoft .NET Framework 4.5.2 CHS installer requested reboot"
            SetRebootFlag true
        ${EndIf}
        SetDetailsPrint lastused
        DetailPrint "Microsoft .NET Framework 4.5.2 CHS installer returned $0"
    ${EndIf}
FunctionEnd

Section -AdditionalIcons
  CreateShortCut "$SMPROGRAMS\�����ҹ���ϵͳ\ж��.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\VisitorManager.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\VisitorManager.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

/******************************
 *  �����ǰ�װ�����ж�ز���  *
 ******************************/

Section Uninstall
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\AForge.Controls.dll"
  Delete "$INSTDIR\AForge.Video.DirectShow.dll"
  Delete "$INSTDIR\AForge.Video.dll"
  Delete "$INSTDIR\Apache.NMS.ActiveMQ.dll"
  Delete "$INSTDIR\Apache.NMS.ActiveMQ.xml"
  Delete "$INSTDIR\Apache.NMS.dll"
  Delete "$INSTDIR\Apache.NMS.xml"
  Delete "$INSTDIR\AsyncIO.dll"
  Delete "$INSTDIR\CodeReason.Reports.dll"
  Delete "$INSTDIR\Config.ini"
  Delete "$INSTDIR\Dapper.dll"
  Delete "$INSTDIR\ExcelReport.dll"
  Delete "$INSTDIR\ICSharpCode.SharpZipLib.dll"
  Delete "$INSTDIR\Microsoft.Expression.Drawing.dll"
  Delete "$INSTDIR\Microsoft.Expression.Drawing.xml"
  Delete "$INSTDIR\Microsoft.Expression.Effects.dll"
  Delete "$INSTDIR\Microsoft.Expression.Effects.xml"
  Delete "$INSTDIR\Microsoft.Expression.Interactions.dll"
  Delete "$INSTDIR\Microsoft.Expression.Interactions.xml"
  Delete "$INSTDIR\Microsoft.Windows.Shell.dll"
  Delete "$INSTDIR\MySql.Data.dll"
  Delete "$INSTDIR\NetMQ.dll"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\NLog.dll"
  Delete "$INSTDIR\NLog.xml"
  Delete "$INSTDIR\NPOI.dll"
  Delete "$INSTDIR\NPOI.OOXML.dll"
  Delete "$INSTDIR\NPOI.OOXML.xml"
  Delete "$INSTDIR\NPOI.OpenXml4Net.dll"
  Delete "$INSTDIR\NPOI.OpenXml4Net.xml"
  Delete "$INSTDIR\NPOI.OpenXmlFormats.dll"
  Delete "$INSTDIR\NPOI.xml"
  Delete "$INSTDIR\PresentationCore.dll"
  Delete "$INSTDIR\PresentationFramework.Aero.dll"
  Delete "$INSTDIR\PresentationFramework.dll"
  Delete "$INSTDIR\System.Data.SQLite.DLL"
  Delete "$INSTDIR\System.Utility.dll"
  Delete "$INSTDIR\System.Windows.Interactivity.dll"
  Delete "$INSTDIR\System.Windows.Interactivity.xml"
  Delete "$INSTDIR\Thrift.dll"
  Delete "$INSTDIR\ThriftCommon.dll"
  Delete "$INSTDIR\VisitorManager.exe"
  Delete "$INSTDIR\VisitorManager.exe.config"
  Delete "$INSTDIR\VisitorManager.ViewModel.dll"
  Delete "$INSTDIR\WPF.Extend.Controls.dll"
  Delete "$INSTDIR\WPF.Extend.dll"

	RMDir /r "$INSTDIR\Doc"
  RMDir /r "$INSTDIR\Image"
  RMDir /r "$INSTDIR\SDK"

  Delete "$SMPROGRAMS\�����ҹ���ϵͳ\Uninstall.lnk"
  Delete "$DESKTOP\�����ҹ���ϵͳ.lnk"
  Delete "$SMPROGRAMS\�����ҹ���ϵͳ\�����ҹ���ϵͳ.lnk"

  RMDir "$SMPROGRAMS\�����ҹ���ϵͳ"

  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd

#-- ���� NSIS �ű��༭�������� Function ���α�������� Section ����֮���д���Ա��ⰲװ�������δ��Ԥ֪�����⡣--#

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "��ȷʵҪ��ȫ�Ƴ� $(^Name) ���������е������" IDYES +2
  Abort
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) �ѳɹ��ش����ļ�����Ƴ���"
FunctionEnd
