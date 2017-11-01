// 这是主 DLL 文件。

#include "stdafx.h"

#include "EfficiencyCLRWrapper.h"  

using namespace EfficiencyCLRWrapper;  
  
CLRWrapper::CLRWrapper()
{
  this->_pNtvCppPro = new EfficiencyNativeCppDll();
}

CLRWrapper::~CLRWrapper()
{
}  
  
double CLRWrapper::ComputePot()  
{  
    return this->_pNtvCppPro->ComputePot();  
}  
  
void CLRWrapper::InitPositions()  
{  
    this->_pNtvCppPro->InitPositions();  
}  
  
void CLRWrapper::UpdatePositions()  
{  
    this->_pNtvCppPro->UpdatePositions();  
}  
  
  
double CLRWrapper::Pot::get()  
{  
    return this->_pNtvCppPro->Pot;  
}  
  
void CLRWrapper::Pot::set(double value)  
{  
    this->_pNtvCppPro->Pot = value;  
}  