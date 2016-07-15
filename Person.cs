using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace MonopolyCSharp
{
    /// <summary>
    /// 人物物件初始資料結構
    /// </summary>
    struct PersonInitData
    {
        /// <summary>委派方法-物件加入主畫面Grid</summary>
        public AddGirdConnect APGCMain;
        /// <summary>委派方法-物件加入資訊欄畫面Grid</summary>
        public AddGirdConnect APGCInformation;
        /// <summary>使用之人物圖片編號</summary>
        public int picNum;
        /// <summary>資訊欄位置編排編號</summary>
        public int InformationXYNum;
    }
    /// <summary>
    /// MonopolyCSharp-人物
    /// </summary>
    class Person
    {
        /// <summary>人物圖片表示-主畫面</summary>
        private Image picme = new Image();
        /// <summary>人物圖片表示-資訊版</summary>
        private Image pichead = new Image();
        /// <summary>人物金錢表示-資訊版</summary>
        private Label LableMoney = new Label();
        /// <summary>人物名稱</summary>
        public string pName { get; set; }
        /// <summary>人物金錢</summary>
        private int pMoney
        {
            get
            {
                return _pmoney;
            }
            set
            {
                _pmoney = value;
                LableMoney.Content = value;
            }
        }
        private int _pmoney=0;
        /// <summary>人物現在地圖站點編號</summary>
        public int pMapNowNum { get; set; }
        /// <summary>人物角色選擇編號</summary>
        public int picNum { get; set; }
        /// <summary>人物行動動畫結束事件</summary>
        public event EventHandler WalkCompleted;
        //===========================
        /// <summary>
        /// 人物-建構式
        /// </summary>
        /// <param name="APGCMain">委派方法-物件加入主畫面Grid</param>
        /// <param name="APGCInformation">委派方法-物件加入資訊欄畫面Grid</param>
        /// <param name="personNum">使用之人物圖片編號</param>
        /// <param name="personXYNum">資訊欄位置編排編號</param>
        public Person(PersonInitData _personInitData)
        {
            personInitMain(_personInitData.APGCMain, _personInitData.picNum);
            personInitInformation(_personInitData.APGCInformation, _personInitData.picNum, _personInitData.InformationXYNum);
        }
        private void personInitMain(AddGirdConnect APGCMain, int _picnum)
        {
            picNum = _picnum;
            BitmapImage bp1= new BitmapImage(new Uri("images/" + _picnum + ".png", UriKind.Relative));
            picme.Source = bp1;
            picme.Stretch = Stretch.Uniform;
            picme.Width = bp1.Width*(60/bp1.Height);
            picme.Height = 60;
            picme.Visibility = Visibility.Visible;
            APGCMain(picme);
            picme.HorizontalAlignment = HorizontalAlignment.Left;
            picme.VerticalAlignment = VerticalAlignment.Top;
            picme.Margin = new Thickness(0, 0, 0, 0);
        }
        private void personInitInformation(AddGirdConnect APGCInformation
            , int _picnum, int InformationXYNum)
        {
            pichead.Source = new BitmapImage(new Uri("images/" + _picnum + "head.png", UriKind.Relative));
            pichead.Stretch = Stretch.Uniform;
            pichead.Width = 50;
            pichead.Height = 50;
            pichead.Visibility = Visibility.Visible;
            APGCInformation(pichead);
            pichead.HorizontalAlignment = HorizontalAlignment.Left;
            pichead.VerticalAlignment = VerticalAlignment.Top;
            pichead.Margin = new Thickness(134* InformationXYNum, 5, 0, 0);
            //===========================
            APGCInformation(LableMoney);
            LableMoney.Content = "999999";
            LableMoney.FontFamily = new FontFamily("Microsoft JhengHei UI");
            LableMoney.FontSize = 16;
            LableMoney.Margin = new Thickness(134 * InformationXYNum + 52, 18, 0, 0);
        }
        /// <summary>
        /// 傳回人物圖片表示物件
        /// </summary>
        public Image GetImageMain() { return picme; }
        /// <summary>
        /// 人物行動動畫
        /// </summary>
        /// <param name="walklong">行動長度</param>
        /// <param name="MapXY">自目前位置到目的地之座標</param>
        public void Walk(int walklong,int[][] MapXY)
        {
            GC.Collect();
            //===========================
            int MsecTot = 0;  //每跳一格多加之總毫秒數
            Storyboard pStoryboard = new Storyboard();  //動畫腳本物件
            ThicknessAnimationUsingKeyFrames pWalkAnimation_TF1 = new ThicknessAnimationUsingKeyFrames();  //(座標式)物件直接動畫方法(指定影格)
            pStoryboard.Completed += new EventHandler(AnimationWalkEvent);
            pWalkAnimation_TF1.KeyFrames.Clear();
            for (int i = 0; i < MapXY.GetUpperBound(0); i++)
            {
                //加入影格-人物移動部分
                MsecTot += 150;
                pWalkAnimation_TF1.KeyFrames.Add(new SplineThicknessKeyFrame(
                    new Thickness(MapXY[i+1][0] - (picme.Width / 2), MapXY[i + 1][1] - (picme.Height / 2), 0, 0), TimeSpan.FromMilliseconds(MsecTot),new KeySpline(0.0,0.2,0.6,0.0)));
                //加入影格-到達後頓點部分
                MsecTot += 50;
                pWalkAnimation_TF1.KeyFrames.Add(new SplineThicknessKeyFrame(
                    new Thickness(MapXY[i + 1][0] - (picme.Width / 2), MapXY[i + 1][1] - (picme.Height / 2), 0, 0), TimeSpan.FromMilliseconds(MsecTot)));
            }
            Storyboard.SetTarget(pWalkAnimation_TF1, picme);
            Storyboard.SetTargetProperty(pWalkAnimation_TF1, new PropertyPath(Image.MarginProperty));
            pStoryboard.Stop();
            pStoryboard.Children.Clear();
            pStoryboard.Children.Add(pWalkAnimation_TF1);
            pStoryboard.Begin(picme);
        }
        /// <summary>
        /// 傳達Walk結束之事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnimationWalkEvent(object sender, EventArgs e)
        {
            if (WalkCompleted != null) WalkCompleted(sender,e);
        }
        /// <summary>
        /// 設定人物座標
        /// </summary>
        /// <param name="X">座標X</param>
        /// <param name="Y">座標Y</param>
        /// <param name="_Num">人物現在地圖站點編號</param>
        public void SetPersonXYN(int X,int Y,int _Num)
        {
            picme.Margin = new Thickness(X-(picme.Width/2), Y - (picme.Height / 2), 0, 0);
            pMapNowNum = _Num;
        }
        /// <summary>
        /// 人物現金-扣除方法
        /// </summary>
        /// <param name="_num">支付金額</param>
        /// <returns>扣除是否成功</returns>
        public bool Money_Pay(int _num)
        {
            if (pMoney >= _num && _num>=0)
            {
                pMoney -= _num;
                return true;
            }
            else return false;
        }
        /// <summary>
        /// 人物現金-存錢方法
        /// </summary>
        /// <param name="_num">存錢金額</param>
        /// <returns>存錢是否成功</returns>
        public bool Money_Save(int _num)
        {
            if ( _num>0)
            {
                pMoney += _num;
                return true;
            }
            else return false;
        }
        /// <summary>
        /// 取得人物現金值
        /// </summary>
        /// <returns>人物現金值</returns>
        public int GetMoneyValue()
        {
            return pMoney;
        }
    }
}
