// EfficiencyCLRWrapper.h

#pragma once  
#include "EfficiencyNativeCppDll.h"  
#define  GoWin_DLL_CLASS  
  
using namespace System;  
  
namespace EfficiencyCLRWrapper {  
  
    public ref class CLRWrapper  
    {  
    private:  
        EfficiencyNativeCppDll * _pNtvCppPro;  
    public:   
        CLRWrapper(void);  
        ~CLRWrapper(void);  
        void InitPositions();  
        void UpdatePositions();  
        double ComputePot();  
        property double Pot  
        {  
        double get();  
        void set(double value);  
        }  
    };  
}  
