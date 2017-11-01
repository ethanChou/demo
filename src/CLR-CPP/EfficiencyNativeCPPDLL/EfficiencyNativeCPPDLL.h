#pragma once  
  
#ifndef GoWin_DLL_CLASS_EXPORTS  
//该类可导出  
#define GoWin_DLL_CLASS __declspec(dllexport)  
#else  
//该类可导入  
#define GoWin_DLL_CLASS __declspec(dllimport)  
#endif  
#define NPARTS 1000  
#define DIMS 3  
  
class GoWin_DLL_CLASS EfficiencyNativeCppDll
{  
public:  
    EfficiencyNativeCppDll(void);  
    ~EfficiencyNativeCppDll(void);  
      
    void InitPositions();  
    void UpdatePositions();  
    double ComputePot();  
    double Pot;  
private:  
    double _r[DIMS][NPARTS];  
};  