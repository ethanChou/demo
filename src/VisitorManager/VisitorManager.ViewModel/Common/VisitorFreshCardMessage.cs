using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorManager.ViewModel
{

    public class LenelCardHolderStruct
    {
        public string ADDR1
        {
            get;
            set;
        }
        public string AlloweDvisitors
        {
            get;
            set;
        }
        public string BDate
        {
            get;
            set;
        }
        public string Building
        {
            get;
            set;
        }
        public string City
        {
            get;
            set;
        }
        public string Dept
        {
            get;
            set;
        }
        public string Division
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public string Ext
        {
            get;
            set;
        }
        public string FirstName
        {
            get;
            set;
        }
        public string Floor
        {
            get;
            set;
        }
        public string ID
        {
            get;
            set;
        }
        public string LastChanged
        {
            get;
            set;
        }
        public string LastName
        {
            get;
            set;
        }
        public string Location
        {
            get;
            set;
        }
        public string MidName
        {
            get;
            set;
        }
        public string Ophone
        {
            get;
            set;
        }
        public string Phone
        {
            get;
            set;
        }
        public string SSNO
        {
            get;
            set;
        }
        public string State
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }
        public string Zip
        {
            get;
            set;
        }

        public LenelCardHolderStruct()
        {
        }

    }

    public class VisitorFreshCardMessage
    {
        public VisitorFreshCardMessage()
        {
        }

        public string card_no { get; set; }
        public LenelCardHolderStruct cardholder { get; set; }
        public CardHolderType cardholder_type { get; set; }
        public string cardReader_Id { get; set; }
        public string data_type { get; set; }
        public string img_url { get; set; }
    }

    public enum CardHolderType
    {
        None = 0,
        CardHolder = 1,
        Visitor = 2,
        Tmproty = 3,
    }
}
