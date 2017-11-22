namespace csharp ThriftCommon
namespace java com.jy.lenelthrift
enum IdentifyType{
	IdCard,
	Employee,
	Ohter
}
enum Status{
	None,
	Visiting,
	Leave,
	NoComeBack,
	LostCard
}
struct Adrelation{
	1:string ad_id,//院所id
	2:string lnl_id,//lenelId
	3:string ad_name,//名称
	4:bool ad_isdeleted,//是否删除
}
struct Department{
	1:string dep_id,
	2:string lnl_id,
	3:string dep_parent_id,//父级Id
	4:string ad_id,
	5:string dep_no,
	6:string dep_name,
	7:bool dep_isspecial,//是否特殊单位
	8:bool dep_isdeleted,
}
struct Employee{
	1:string emp_id,
	2:string lnl_id,
	3:string emp_no,
	4:string emp_name,
	5:string emp_sex,
	6:string dep_id,
	7:string card_no,
	8:string emp_tel,
	9:string emp_imgurl,
	10:bool emp_isdeleted,
}
struct VisitorList{
	1:string vl_id,
	2:i64 vl_in_time,
	3:i64 vl_out_time,
	4:string vl_carryThings
}
struct Visitor{
	1:string vt_id,
	2:string vt_name,
	3:string vt_sex,
	4:IdentifyType vt_identify_type,//1.身份证，2员工卡，3其他
	5:string vt_identify_no,
	6:string vt_identify_imgurl,
	7:string vt_imgurl,
	8:string tmpcard_no,
	9:i64 vt_in_time,
	10:i64 vt_out_time,
	11:Status vt_status,//1.正在访问，2.已经离开，3逾期未还
	12:string vt_visit_department_id,//访问单位id,可为空
	13:string vt_visit_employee_id,//被访问人员id,可为空
	14:string vt_vl_id//单号
}
struct BlackList{
	1:string bl_id,
	2:IdentifyType bl_identify_type,//1.身份证，2员工卡，3其他
	3:string bl_identify_no,
	4:string bl_name,
	5:string bl_sex,
	6:string bl_reason//加入黑名单的缘由
	7:bool bl_isdeleted,
}
struct ReaderList{
	1:string reader_id,
	2:string reader_name
}

enum DbOprator{
	Add,
	Update,
	Delete
}
service LenelDataService {
	list<ReaderList> GetReaders();
	bool SetVisitorReader(1:list<ReaderList> readers);
	bool SetFaceReader(1:list<ReaderList> readers);
	bool AdrelationOprator(1:DbOprator dboprator,2: Adrelation adrelation);
	list<Adrelation> GetAdralations(1:string id,2:string lnl_id,3:string name);

	bool DepartmentOprator(1:DbOprator dboprator,2: Department department);
	list<Department> GetDepartments(1:string ad_id,2:string id,3:string lnl_id,4:string name);

	bool EmployeeOprator(1:DbOprator dboprator,2: Employee employee);
	list<Employee> GetEmployees(1:string dep_id,2:string id,3:string lnl_id,4:string name,5:string card_NO);

	bool AddVisitorList(1:VisitorList vlist);
	bool UpdateVisitorList(1:VisitorList vlist);
	list<VisitorList> GetVisitorLists(1:string vl_id,2:i64 vl_in_time=0,3:i64 vl_out_time=0);
	bool DeleteVisitorList(1:string vl_id);

	bool AddVisitor(1:list<Visitor> visitors);
	bool UpdateVisitor(1:Visitor visitor);
	list<Visitor> GetVisitors(1:string vt_id,2:string vt_vl_id,3:string name,4:IdentifyType identify_type=0,5:string tmpcard_no,6:string vt_identify_NO,7:i64 in_time,8:i64 out_time,9:Status status,10:string dep_id,11:string emp_id);
	bool DeleteVisitor(1:string vt_id);

	bool BlackListOprator(1:DbOprator dboprator,2:BlackList blacklist);
	list<BlackList> GetBlackList(1:string bl_id,2:IdentifyType identify_type=0,3:string bl_identify_NO,4:string name);
	string UploadImg2Bimg(1:binary imgBytes);#图片上传至bimg,返回上传路径
}