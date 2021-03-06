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
  public partial class Employee : TBase
  {
    private string _emp_id;
    private string _lnl_id;
    private string _emp_no;
    private string _emp_name;
    private string _emp_sex;
    private string _dep_id;
    private string _card_no;
    private string _emp_tel;
    private string _emp_imgurl;
    private bool _emp_isdeleted;

    public string Emp_id
    {
      get
      {
        return _emp_id;
      }
      set
      {
        __isset.emp_id = true;
        this._emp_id = value;
      }
    }

    public string Lnl_id
    {
      get
      {
        return _lnl_id;
      }
      set
      {
        __isset.lnl_id = true;
        this._lnl_id = value;
      }
    }

    public string Emp_no
    {
      get
      {
        return _emp_no;
      }
      set
      {
        __isset.emp_no = true;
        this._emp_no = value;
      }
    }

    public string Emp_name
    {
      get
      {
        return _emp_name;
      }
      set
      {
        __isset.emp_name = true;
        this._emp_name = value;
      }
    }

    public string Emp_sex
    {
      get
      {
        return _emp_sex;
      }
      set
      {
        __isset.emp_sex = true;
        this._emp_sex = value;
      }
    }

    public string Dep_id
    {
      get
      {
        return _dep_id;
      }
      set
      {
        __isset.dep_id = true;
        this._dep_id = value;
      }
    }

    public string Card_no
    {
      get
      {
        return _card_no;
      }
      set
      {
        __isset.card_no = true;
        this._card_no = value;
      }
    }

    public string Emp_tel
    {
      get
      {
        return _emp_tel;
      }
      set
      {
        __isset.emp_tel = true;
        this._emp_tel = value;
      }
    }

    public string Emp_imgurl
    {
      get
      {
        return _emp_imgurl;
      }
      set
      {
        __isset.emp_imgurl = true;
        this._emp_imgurl = value;
      }
    }

    public bool Emp_isdeleted
    {
      get
      {
        return _emp_isdeleted;
      }
      set
      {
        __isset.emp_isdeleted = true;
        this._emp_isdeleted = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool emp_id;
      public bool lnl_id;
      public bool emp_no;
      public bool emp_name;
      public bool emp_sex;
      public bool dep_id;
      public bool card_no;
      public bool emp_tel;
      public bool emp_imgurl;
      public bool emp_isdeleted;
    }

    public Employee() {
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
                Emp_id = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 2:
              if (field.Type == TType.String) {
                Lnl_id = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 3:
              if (field.Type == TType.String) {
                Emp_no = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 4:
              if (field.Type == TType.String) {
                Emp_name = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 5:
              if (field.Type == TType.String) {
                Emp_sex = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 6:
              if (field.Type == TType.String) {
                Dep_id = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 7:
              if (field.Type == TType.String) {
                Card_no = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 8:
              if (field.Type == TType.String) {
                Emp_tel = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 9:
              if (field.Type == TType.String) {
                Emp_imgurl = iprot.ReadString();
              } else { 
                TProtocolUtil.Skip(iprot, field.Type);
              }
              break;
            case 10:
              if (field.Type == TType.Bool) {
                Emp_isdeleted = iprot.ReadBool();
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
        TStruct struc = new TStruct("Employee");
        oprot.WriteStructBegin(struc);
        TField field = new TField();
        if (Emp_id != null && __isset.emp_id) {
          field.Name = "emp_id";
          field.Type = TType.String;
          field.ID = 1;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Emp_id);
          oprot.WriteFieldEnd();
        }
        if (Lnl_id != null && __isset.lnl_id) {
          field.Name = "lnl_id";
          field.Type = TType.String;
          field.ID = 2;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Lnl_id);
          oprot.WriteFieldEnd();
        }
        if (Emp_no != null && __isset.emp_no) {
          field.Name = "emp_no";
          field.Type = TType.String;
          field.ID = 3;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Emp_no);
          oprot.WriteFieldEnd();
        }
        if (Emp_name != null && __isset.emp_name) {
          field.Name = "emp_name";
          field.Type = TType.String;
          field.ID = 4;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Emp_name);
          oprot.WriteFieldEnd();
        }
        if (Emp_sex != null && __isset.emp_sex) {
          field.Name = "emp_sex";
          field.Type = TType.String;
          field.ID = 5;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Emp_sex);
          oprot.WriteFieldEnd();
        }
        if (Dep_id != null && __isset.dep_id) {
          field.Name = "dep_id";
          field.Type = TType.String;
          field.ID = 6;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Dep_id);
          oprot.WriteFieldEnd();
        }
        if (Card_no != null && __isset.card_no) {
          field.Name = "card_no";
          field.Type = TType.String;
          field.ID = 7;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Card_no);
          oprot.WriteFieldEnd();
        }
        if (Emp_tel != null && __isset.emp_tel) {
          field.Name = "emp_tel";
          field.Type = TType.String;
          field.ID = 8;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Emp_tel);
          oprot.WriteFieldEnd();
        }
        if (Emp_imgurl != null && __isset.emp_imgurl) {
          field.Name = "emp_imgurl";
          field.Type = TType.String;
          field.ID = 9;
          oprot.WriteFieldBegin(field);
          oprot.WriteString(Emp_imgurl);
          oprot.WriteFieldEnd();
        }
        if (__isset.emp_isdeleted) {
          field.Name = "emp_isdeleted";
          field.Type = TType.Bool;
          field.ID = 10;
          oprot.WriteFieldBegin(field);
          oprot.WriteBool(Emp_isdeleted);
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
      StringBuilder __sb = new StringBuilder("Employee(");
      bool __first = true;
      if (Emp_id != null && __isset.emp_id) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Emp_id: ");
        __sb.Append(Emp_id);
      }
      if (Lnl_id != null && __isset.lnl_id) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Lnl_id: ");
        __sb.Append(Lnl_id);
      }
      if (Emp_no != null && __isset.emp_no) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Emp_no: ");
        __sb.Append(Emp_no);
      }
      if (Emp_name != null && __isset.emp_name) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Emp_name: ");
        __sb.Append(Emp_name);
      }
      if (Emp_sex != null && __isset.emp_sex) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Emp_sex: ");
        __sb.Append(Emp_sex);
      }
      if (Dep_id != null && __isset.dep_id) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Dep_id: ");
        __sb.Append(Dep_id);
      }
      if (Card_no != null && __isset.card_no) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Card_no: ");
        __sb.Append(Card_no);
      }
      if (Emp_tel != null && __isset.emp_tel) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Emp_tel: ");
        __sb.Append(Emp_tel);
      }
      if (Emp_imgurl != null && __isset.emp_imgurl) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Emp_imgurl: ");
        __sb.Append(Emp_imgurl);
      }
      if (__isset.emp_isdeleted) {
        if(!__first) { __sb.Append(", "); }
        __first = false;
        __sb.Append("Emp_isdeleted: ");
        __sb.Append(Emp_isdeleted);
      }
      __sb.Append(")");
      return __sb.ToString();
    }

  }

}
