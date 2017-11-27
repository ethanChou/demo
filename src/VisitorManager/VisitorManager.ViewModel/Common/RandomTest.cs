using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VisitorManager.ViewModel
{
    public class RandomTest
    {
        private static string[] telStarts = "134,135,136,137,138,139,150,151,152,157,158,159,130,131,132,155,156,133,153,180,181,182,183,185,186,176,187,188,189,177,178".Split(',');
        private static Random ran = new Random();
        /// <summary>

        /// 随机生成电话号码

        /// </summary>

        /// <returns></returns>

        public static string getRandomTel()
        {

            int n = ran.Next(10, 1000);

            int index = ran.Next(0, telStarts.Length - 1);

            string first = telStarts[index];

            string second = (ran.Next(100, 888) + 10000).ToString().Substring(1);

            string thrid = (ran.Next(1, 9100) + 10000).ToString().Substring(1);

            return first + second + thrid;

        }

        private static string firstName = @"赵,钱,孙,李,周,吴,郑,王,冯,陈,褚,卫,蒋,
            沈,韩,杨,朱,秦,尤,许,何,吕,施,张,孔,曹,严,华,金,魏,陶,姜, 戚,谢,邹,喻,
            柏,水,窦,章,云,苏,潘,葛,奚,范,彭,郎,鲁,韦,昌,马,苗,凤,花,方,俞,任,袁,柳,
            丰,鲍,史,唐, 费,廉,岑,薛,雷,贺,倪,汤,滕,殷,罗,毕,郝,邬,安,常,乐,于,时,
            傅,皮,卞,齐,康,伍,余,元,卜,顾,孟,平,黄, 和,穆,萧,尹,姚,邵,湛,汪,祁,毛,
            禹,狄,米,贝,明,臧,计,伏,成,戴,谈,宋,茅,庞,熊,纪,舒,屈,项,祝,董,梁, 杜,
            阮,蓝,闵,席,季,麻,强,贾,路,娄,危,江,童,颜,郭,梅,盛,林,刁,钟,徐,丘,骆,高,
            夏,蔡,田,樊,胡,凌,霍, 虞,万,支,柯,昝,管,卢,莫,经,房,裘,缪,干,解,应,宗,丁,
            宣,贲,邓,郁,单,杭,洪,包,诸,左,石,崔,吉,钮,龚, 程,嵇,邢,滑,裴,陆,荣,翁,荀,
            羊,於,惠,甄,麴,家,封,芮,羿,储,靳,汲,邴,糜,松,井,段,富,巫,乌,焦,巴,弓, 牧,
            隗,山,谷,车,侯,宓,蓬,全,郗,班,仰,秋,仲,伊,宫,宁,仇,栾,暴,甘,钭,厉,戌,祖,
            武,符,刘,景,詹,束,龙, 叶,幸,司,韶,郜,黎,蓟,薄,印,宿,白,怀,蒲,邰,从,鄂,索,
            咸,籍,赖,卓,蔺,屠,蒙,池,乔,阴,郁,胥,能,苍,双, 闻,莘,党,翟,谭,贡,劳,逢,姬,
            申,扶,堵,冉,宰,郦,雍,郤,璩,桑,桂,濮,牛,寿,通,边,扈,燕,冀,郏,浦,尚,农, 温,
            别,庄,晏,柴,瞿,阎,充,慕,连,茹,习,宦,艾,鱼,容,向,古,易,慎,戈,廖,庾,终,暨,
            居,衡,步,都,耿,满,弘, 匡,国,文,寇,广,禄,阙,东,欧,殳,沃,利,蔚,越,菱,隆,师,
            巩,厍,聂,晃,勾,敖,融,冷,訾,辛,阚,那,简,饶,空, 曾,毋,沙,乜,养,鞠,须,丰,巢,
            关,蒯,相,查,后,荆,红,游,竺,权,逯,盖,益,桓,公, 万俟,司马,上官,欧阳,夏侯,
            诸葛,闻人,东方,赫连,皇甫,尉迟,公羊,澹台,公冶,宗政,濮阳,淳于,单于,太叔,
            申屠,公孙,仲孙,轩辕,令狐,钟离,宇文,长孙,慕容,司徒,司空";

        private static string lastName = @"努,迪,立,林,维,吐,丽,新,涛,米,亚,克,湘,明,
            白,玉,代,孜,霖,霞,加,永,卿,约,小,刚,光,峰,春,基,木,国,娜,晓,兰,阿,伟,英,元,
            音,拉,亮,玲,木,兴,成,尔,远,东,华,旭,迪,吉,高,翠,莉,云,华,军,荣,柱,科,生,昊,
            耀,汤,胜,坚,仁,学,荣,延,成,庆,音,初,杰,宪,雄,久,培,祥,胜,梅,顺,涛,西,库,康,
            温,校,信,志,图,艾,赛,潘,多,振,伟,继,福,柯,雷,田,也,勇,乾,其,买,姚,杜,关,陈,
            静,宁,春,马,德,水,梦,晶,精,瑶,朗,语,日,月,星,河,飘,渺,星,空,如,萍,棕,影,南,北";
        private static string nationName = @"汉族,壮族,满族,回族,苗族,维吾尔族,土家族,
            彝族,蒙古族,藏族,布依族,侗族,瑶族,朝鲜族,白族,哈尼族,哈萨克族,黎族,傣族,畲族,
            傈僳族,仡佬族,东乡族,高山族,拉祜族,水族,佤族,纳西族,羌族,土族,仫佬族,锡伯族,
            柯尔克孜族,达斡尔族,景颇族,毛南族,撒拉族,布朗族,塔吉克族,阿昌族,普米族,鄂温克族
            ,怒族,京族,基诺族,德昂族,保安族,俄罗斯族,裕固族,乌兹别克族,门巴族,鄂伦春族,
            独龙族,塔塔尔族,赫哲族,珞巴族";
        static Random rnd = new Random((int)DateTime.Now.ToFileTimeUtc());

        public static string getRandomName()
        {
            int namelength = 0;
            namelength = rnd.Next(2, 4);
            firstName = firstName.Replace("\n", "");
            firstName = firstName.Replace("\r", "");
            firstName = firstName.Replace(" ", "");
            lastName = lastName.Replace("\r", "");
            lastName = lastName.Replace("\n", "");
            lastName = lastName.Replace(" ", "");
            string name = "";
            string[] FirstName = firstName.Split(',');
            string[] LastName = lastName.Split(',');
            if (namelength == 2)
            {
                name = FirstName[rnd.Next(0, FirstName.Length)] + LastName[rnd.Next(0, LastName.Length)];
            }
            else if (namelength == 3)
            {
                name = FirstName[rnd.Next(0, FirstName.Length)] + LastName[rnd.Next(0, LastName.Length)] + LastName[rnd.Next(0, LastName.Length)];
            }

            return name;
        }
        public static string GetRandomNumber(int startnumber, int endnumber)
        {
            string strNumber = rnd.Next(startnumber, endnumber).ToString();
            return strNumber;
        }
        public static string GetRandomNation()
        {
            nationName = nationName.Replace("\n", "");
            nationName = nationName.Replace("\r", "");
            nationName = nationName.Replace(" ", "");
            string[] nationname = nationName.Split(',');
            string nation = nationname[rnd.Next(0, 55)];
            return nation;
        }
    }


    /// <summary>
    /// 身份证号码解析与生成
    /// 作者：宋雷鸣 10522779@qq.com
    /// </summary>
    public class IDCardNumber
    {

        public static string GenPinCode()
        {
            System.Random rnd;
            string[] _crabodistrict = new string[] { "350201", "350202", "350203", "350204", "350205", "350206", "350211", "350205", "350213" };

            rnd = new Random(System.DateTime.Now.Millisecond);

            //PIN = District + Year(50-92) + Month(01-12) + Date(01-30) + Seq(001-600)
            string _pinCode = string.Format("{0}19{1}{2:00}{3:00}{4:000}", _crabodistrict[rnd.Next(0, 8)], rnd.Next(50, 92), rnd.Next(1, 12), rnd.Next(1, 30), rnd.Next(1, 600));
            #region Verify
            char[] _chrPinCode = _pinCode.ToCharArray();
            //校验码字符值
            char[] _chrVerify = new char[] { '1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
            //i----表示号码字符从由至左包括校验码在内的位置序号；
            //ai----表示第i位置上的号码字符值；
            //Wi----示第i位置上的加权因子，其数值依据公式intWeight=2（n-1）(mod 11)计算得出。
            int[] _intWeight = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
            int _craboWeight = 0;
            for (int i = 0; i < 17; i++)//从1 到 17 位,18为要生成的验证码
            {
                _craboWeight = _craboWeight + Convert.ToUInt16(_chrPinCode[i].ToString()) * _intWeight[i];
            }
            _craboWeight = _craboWeight % 11;
            _pinCode += _chrVerify[_craboWeight];
            #endregion
            return _pinCode;
        }

        #region 身份证信息属性
        private string _province;
        /// <summary>
        /// 所在省份信息
        /// </summary>
        public string Province
        {
            get { return _province; }
            set { _province = value; }
        }
        private string _area;
        /// <summary>
        /// 所在地区信息
        /// </summary>
        public string Area
        {
            get { return _area; }
            set { _area = value; }
        }
        private string _city;
        /// <summary>
        /// 所在区县信息
        /// </summary>
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }
        private DateTime _age;
        /// <summary>
        /// 年龄
        /// </summary>
        public DateTime Age
        {
            get { return _age; }
            set { _age = value; }
        }
        private int _sex;
        /// <summary>
        /// 性别，0为女，1为男
        /// </summary>
        public int Sex
        {
            get { return _sex; }
            set { _sex = value; }
        }
        private string _cardnumber;
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string CardNumber
        {
            get { return _cardnumber; }
            set { _cardnumber = value; }
        }
        private string _json;
        /// <summary>
        /// 生成Javascript对象；
        /// </summary>
        public string Json
        {
            get { return _json; }
            set { _json = value; }
        }

        #endregion

        #region 静态方法
        private static readonly List<string[]> Areas = new List<string[]>() { new string[] { "350201", "350202", "350203", "350204", "350205", "350206", "350211", "350205", "350213" } };


        /// <summary>
        /// 校验身份证号码是否合法
        /// </summary>
        /// <param name="idCardNumber"></param>
        /// <returns></returns>
        public static bool CheckIDCardNumber(string idCardNumber)
        {
            //正则验证
            Regex rg = new Regex(@"^\d{17}(\d|X)$");
            Match mc = rg.Match(idCardNumber);
            if (!mc.Success) return false;
            //加权码
            string code = idCardNumber.Substring(17, 1);
            double sum = 0;
            string checkCode = null;
            for (int i = 2; i <= 18; i++)
            {
                sum += int.Parse(idCardNumber[18 - i].ToString(), NumberStyles.HexNumber) * (Math.Pow(2, i - 1) % 11);
            }
            string[] checkCodes = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
            checkCode = checkCodes[(int)sum % 11];
            if (checkCode != code) return false;
            //
            return true;
        }
        /// <summary>
        /// 随机生成一个身份证号
        /// </summary>
        /// <returns></returns>
        public static IDCardNumber Radom()
        {
            long tick = DateTime.Now.Ticks;
            return new IDCardNumber(_radomCardNumber((int)tick));
        }
        /// <summary>
        /// 批量生成身份证
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<IDCardNumber> Radom(int count)
        {
            List<IDCardNumber> list = new List<IDCardNumber>();
            string cardNumber;
            bool isExits;
            for (int i = 0; i < count; i++)
            {
                do
                {
                    isExits = false;
                    int tick = (int)DateTime.Now.Ticks;
                    cardNumber = IDCardNumber._radomCardNumber(tick * (i + 1));
                    foreach (IDCardNumber c in list)
                    {
                        if (c.CardNumber == cardNumber)
                        {
                            isExits = true;
                            break;
                        }
                    }

                } while (isExits);
                list.Add(new IDCardNumber(cardNumber));
            }
            return list;
        }
        /// <summary>
        /// 生成随身份证号
        /// </summary>
        /// <param name="seed">随机数种子</param>
        /// <returns></returns>
        private static string _radomCardNumber(int seed)
        {

            System.Random rd = new System.Random(seed);
            //随机生成发证地
            string area = "";
            do
            {
                area = IDCardNumber.Areas[rd.Next(0, IDCardNumber.Areas.Count - 1)][0];
            } while (area.Substring(4, 2) == "00");
            //随机出生日期
            DateTime birthday = DateTime.Now;
            birthday = birthday.AddYears(-rd.Next(16, 60));
            birthday = birthday.AddMonths(-rd.Next(0, 12));
            birthday = birthday.AddDays(-rd.Next(0, 31));
            //随机码
            string code = rd.Next(1000, 9999).ToString("####");
            //生成完整身份证号
            string codeNumber = area + birthday.ToString("yyyyMMdd") + code;
            double sum = 0;
            string checkCode = null;
            for (int i = 2; i <= 18; i++)
            {
                sum += int.Parse(codeNumber[18 - i].ToString(), NumberStyles.HexNumber) * (Math.Pow(2, i - 1) % 11);
            }
            string[] checkCodes = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
            checkCode = checkCodes[(int)sum % 11];
            codeNumber = codeNumber.Substring(0, 17) + checkCode;
            //
            return codeNumber;
        }
        #endregion

        #region 身份证解析方法

        private IDCardNumber(string idCardNumber)
        {
            this._cardnumber = idCardNumber;
            _analysis();
        }
        /// <summary>
        /// 解析身份证
        /// </summary>
        private void _analysis()
        {
            //取省份，地区，区县
            string provCode = _cardnumber.Substring(0, 2).PadRight(6, '0');
            string areaCode = _cardnumber.Substring(0, 4).PadRight(6, '0');
            string cityCode = _cardnumber.Substring(0, 6).PadRight(6, '0');
            for (int i = 0; i < IDCardNumber.Areas.Count; i++)
            {
                if (provCode == IDCardNumber.Areas[i][0])
                    this._province = IDCardNumber.Areas[i][1];
                if (areaCode == IDCardNumber.Areas[i][0])
                    this._area = IDCardNumber.Areas[i][1];
                if (cityCode == IDCardNumber.Areas[i][0])
                    this._city = IDCardNumber.Areas[i][1];
                if (_province != null && _area != null && _city != null) break;
            }
            //取年龄
            string ageCode = _cardnumber.Substring(6, 8);
            try
            {
                int year = Convert.ToInt16(ageCode.Substring(0, 4));
                int month = Convert.ToInt16(ageCode.Substring(4, 2));
                int day = Convert.ToInt16(ageCode.Substring(6, 2));
                _age = new DateTime(year, month, day);
            }
            catch
            {
                throw new Exception("非法的出生日期");
            }
            //取性别
            string orderCode = _cardnumber.Substring(14, 3);
            this._sex = Convert.ToInt16(orderCode) % 2 == 0 ? 0 : 1;
            //生成Javascript对象
            _json = @"prov:'{0}',area:'{1}',city:'{2}',year:{3},month:{4},day:{5},sex:{6},number:'{7}'";
            _json = string.Format(_json, _province, _area, _city, _age.Year, _age.Month, _age.Day, _sex, _cardnumber);
            _json = "{" + _json + "}";
        }
        #endregion
    }
}
