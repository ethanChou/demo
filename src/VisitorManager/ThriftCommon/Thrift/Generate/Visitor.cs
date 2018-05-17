/**
 * Autogenerated by Thrift Compiler (0.9.3)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace ThriftCommon
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class Visitor : TBase
  {
    private string _vt_id;
    private string _vt_name;
    private string _vt_sex;
    private IdentifyType _vt_identify_type;
    private string _vt_identify_no;
    private string _vt_identify_imgurl;
    private string _vt_imgurl;
    private string _tmpcard_no;
    private long _vt_in_time;
    private long _vt_out_time;
    private Status _vt_status;
    private string _vt_visit_department_id;
    private string _vt_visit_employee_id;
    private string _vt_vl_id;

    public string Vt_id
    {
      get
      {
        return _vt_id;
      }
      set
      {
        __isset.vt_id = true;
        this._vt_id = value;
      }
    }

    public string Vt_name
    {
      get
      {
        return _vt_name;
      }
      set
      {
        __isset.vt_name = true;
        this._vt_name = value;
      }
    }

    public string Vt_sex
    {
      get
      {
        return _vt_sex;
      }
      set
      {
        __isset.vt_sex = true;
        this._vt_sex = value;
      }
    }

    /// <summary>
    /// 
    /// <seealso cref="IdentifyType"/>
    /// </summary>
    public IdentifyType Vt_identify_type
    {
      get
      {
        return _vt_identify_type;
      }
      set
      {
        __isset.vt_identify_type = true;
        this._vt_identify_type = value;
      }
    }

    public string Vt_identify_no
    {
      get
      {
        return _vt_identify_no;
      }
      set
      {
        __isset.vt_identify_no = true;
        this._vt_identify_no = value;
      }
    }

    public string Vt_identify_imgurl
    {
      get
      {
        return _vt_identify_imgurl;
      }
      set
      {
        __isset.vt_identify_imgurl = true;
        this._vt_identify_imgurl = value;
      }
    }

    public string Vt_imgurl
    {
      get
      {
        return _vt_imgurl;
      }
      set
      {
        __isset.vt_imgurl = true;
        this._vt_imgurl = value;
      }
    }

    public string Tmpcard_no
    {
      get
      {
        return _tmpcard_no;
      }
      set
      {
        __isset.tmpcard_no = true;
        this._tmpcard_no = value;
      }
    }

    public long Vt_in_time
    {
      get
      {
        return _vt_in_time;
      }
      set
      {
        __isset.vt_in_time = true;
        this._vt_in_time = value;
      }
    }

    public long Vt_out_time
    {
      get
      {
        return _vt_out_time;
      }
      set
      {
        __isset.vt_out_time = true;
        this._vt_out_time = value;
      }
    }

    /// <summary>
    /// 
    /// <seealso cref="Status"/>
    /// </summary>
    public Status Vt_status
    {
      get
      {
        return _vt_status;
      }
      set
      {
        __isset.vt_status = true;
        this._vt_status = value;
      }
    }

    public string Vt_visit_department_id
    {
      get
      {
        return _vt_visit_department_id;
      }
      set
      {
        __isset.vt_visit_department_id = true;
        this._vt_visit_department_id = value;
      }
    }

    public string Vt_visit_employee_id
    {
      get
      {
        return _vt_visit_employee_id;
      }
      set
      {
        __isset.vt_visit_employee_id = true;
        this._vt_visit_employee_id = value;
      }
    }

    public string Vt_vl_id
    {
      get
      {
        return _vt_vl_id;
      }
      set
      {
        __isset.vt_vl_id = true;
        this._vt_vl_id = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool vt_id;
      public bool vt_name;
      public bool vt_sex;
      public bool vt_identify_type;
      public bool vt_identify_no;
      public bool vt_identify_imgurl;
      public bool vt_imgurl;
      public bool tmpcard_no;
      public bool vt_in_time;
      public bool vt_out_time;
      public bool vt_status;
      public bool vt_visit_department_id;
      public bool vt_visit_employee_id;
      public bool vt_vl_id;
    }

    public Visitor() {
    }

    public void Read (TProtocol iprot)
    {
      iprot.IncrementRecursionDepth();
      try
      {
        TField field;
        iprot.ReadStructBegin();
        while (true)
        {
          field = iprot.ReadFieldBegin();
          if (field.Type == TType.Stop) { 
            break;
          }
          switch (field.ID)
          {
            case 1:
              if (field.Type == TType.String) {
                Vt_id = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.String) {
                Vt_name = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 3:
              if (field.Type == TType.String) {
                Vt_sex = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 4:
              if (field.Type == TType.I32) {
                Vt_identify_type = (IdentifyType)iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 5:
              if (field.Type == TType.String) {
                Vt_identify_no = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 6:
              if (field.Type == TType.String) {
                Vt_identify_imgurl = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 7:
              if (field.Type == TType.String) {
                Vt_imgurl = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 8:
              if (field.Type == TType.String) {
                Tmpcard_no = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 9:
              if (field.Type == TType.I64) {
                Vt_in_time = iprot.ReadI64();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 10:
              if (field.Type == TType.I64) {
                Vt_out_time = iprot.ReadI64();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 11:
              if (field.Type == TType.I32) {
                Vt_status = (Status)iprot.ReadI32();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 12:
              if (field.Type == TType.String) {
                Vt_visit_department_id = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 13:
              if (field.Type == TType.String) {
                Vt_visit_employee_id = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 14:
              if (field.Type == TType.String) {
                Vt_vl_id = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            default: 
              TProtocolUtil.Skip(iprot, field.Type);
              break;
          }
          iprot.ReadFieldEnd();
        }
        iprot.ReadStructEnd();
      }
      finally
      {
        iprot.DecrementRecursionDepth();
      }
    }

    public void Write(TProtocol oprot) {
      oprot.IncrementRecursionDepth();
      try
      {
        TStruct struc = new TStruct("Visitor");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (Vt_id != null && __isset.vt_id) {
          field.Name = "vt_id";
          field.Type = TType.String;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Vt_id);
          oprot.WriteFieldEnd();
        }
        if (Vt_name != null && __isset.vt_name) {
          field.Name = "vt_name";
          field.Type = TType.String;
          field.ID = 2;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Vt_name);
          oprot.WriteFieldEnd();
        }
        if (Vt_sex != null && __isset.vt_sex) {
          field.Name = "vt_sex";
          field.Type = TType.String;
          field.ID = 3;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Vt_sex);
          oprot.WriteFieldEnd();
        }
        if (__isset.vt_identify_type) {
          field.Name = "vt_identify_type";
          field.Type = TType.I32;
          field.ID = 4;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32((int)Vt_identify_type);
          oprot.WriteFieldEnd();
        }
        if (Vt_identify_no != null && __isset.vt_identify_no) {
          field.Name = "vt_identify_no";
          field.Type = TType.String;
          field.ID = 5;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Vt_identify_no);
          oprot.WriteFieldEnd();
        }
        if (Vt_identify_imgurl != null && __isset.vt_identify_imgurl) {
          field.Name = "vt_identify_imgurl";
          field.Type = TType.String;
          field.ID = 6;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Vt_identify_imgurl);
          oprot.WriteFieldEnd();
        }
        if (Vt_imgurl != null && __isset.vt_imgurl) {
          field.Name = "vt_imgurl";
          field.Type = TType.String;
          field.ID = 7;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Vt_imgurl);
          oprot.WriteFieldEnd();
        }
        if (Tmpcard_no != null && __isset.tmpcard_no) {
          field.Name = "tmpcard_no";
          field.Type = TType.String;
          field.ID = 8;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Tmpcard_no);
          oprot.WriteFieldEnd();
        }
        if (__isset.vt_in_time) {
          field.Name = "vt_in_time";
          field.Type = TType.I64;
          field.ID = 9;
          oprot.WriteFieldBegin(field);
          oprot.WriteI64(Vt_in_time);
          oprot.WriteFieldEnd();
        }
        if (__isset.vt_out_time) {
          field.Name = "vt_out_time";
          field.Type = TType.I64;
          field.ID = 10;
          oprot.WriteFieldBegin(field);
          oprot.WriteI64(Vt_out_time);
          oprot.WriteFieldEnd();
        }
        if (__isset.vt_status) {
          field.Name = "vt_status";
          field.Type = TType.I32;
          field.ID = 11;
          oprot.WriteFieldBegin(field);
          oprot.WriteI32((int)Vt_status);
          oprot.WriteFieldEnd();
        }
        if (Vt_visit_department_id != null && __isset.vt_visit_department_id) {
          field.Name = "vt_visit_department_id";
          field.Type = TType.String;
          field.ID = 12;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Vt_visit_department_id);
          oprot.WriteFieldEnd();
        }
        if (Vt_visit_employee_id != null && __isset.vt_visit_employee_id) {
          field.Name = "vt_visit_employee_id";
          field.Type = TType.String;
          field.ID = 13;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Vt_visit_employee_id);
          oprot.WriteFieldEnd();
        }
        if (Vt_vl_id != null && __isset.vt_vl_id) {
          field.Name = "vt_vl_id";
          field.Type = TType.String;
          field.ID = 14;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Vt_vl_id);
          oprot.WriteFieldEnd();
        }
        oprot.WriteFieldStop();
        oprot.WriteStructEnd();
      }
      finally
      {
        oprot.DecrementRecursionDepth();
      }
    }

    public override string ToString() {
      StringBuilder __sb = new StringBuilder("Visitor(");
      bool __first = true;
      if (Vt_id != null && __isset.vt_id) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_id: ");
        __sb.Append(Vt_id);
      }
      if (Vt_name != null && __isset.vt_name) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_name: ");
        __sb.Append(Vt_name);
      }
      if (Vt_sex != null && __isset.vt_sex) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_sex: ");
        __sb.Append(Vt_sex);
      }
      if (__isset.vt_identify_type) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_identify_type: ");
        __sb.Append(Vt_identify_type);
      }
      if (Vt_identify_no != null && __isset.vt_identify_no) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_identify_no: ");
        __sb.Append(Vt_identify_no);
      }
      if (Vt_identify_imgurl != null && __isset.vt_identify_imgurl) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_identify_imgurl: ");
        __sb.Append(Vt_identify_imgurl);
      }
      if (Vt_imgurl != null && __isset.vt_imgurl) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_imgurl: ");
        __sb.Append(Vt_imgurl);
      }
      if (Tmpcard_no != null && __isset.tmpcard_no) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Tmpcard_no: ");
        __sb.Append(Tmpcard_no);
      }
      if (__isset.vt_in_time) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_in_time: ");
        __sb.Append(Vt_in_time);
      }
      if (__isset.vt_out_time) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_out_time: ");
        __sb.Append(Vt_out_time);
      }
      if (__isset.vt_status) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_status: ");
        __sb.Append(Vt_status);
      }
      if (Vt_visit_department_id != null && __isset.vt_visit_department_id) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_visit_department_id: ");
        __sb.Append(Vt_visit_department_id);
      }
      if (Vt_visit_employee_id != null && __isset.vt_visit_employee_id) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_visit_employee_id: ");
        __sb.Append(Vt_visit_employee_id);
      }
      if (Vt_vl_id != null && __isset.vt_vl_id) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Vt_vl_id: ");
        __sb.Append(Vt_vl_id);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}