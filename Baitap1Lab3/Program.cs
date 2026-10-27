
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace BaiTapHinhHoc
{
    class HinhHoc
    {
        public double DienTich { get; protected set; }
        public double ChuVi { get; protected set; }

        public virtual void tinhDienTichChuVi()
        {
            DienTich = 0;
            ChuVi = 0;
        }

        public override string ToString()
        {
            return $"Dien tich: {DienTich:F2}, Chu vi: {ChuVi:F2}";
        }
    }

    class HinhChuNhat : HinhHoc
    {
        private double mChieuDai;
        private double mChieuRong;

        public double ChieuDai
        {
            get { return mChieuDai; }
            set { mChieuDai = value; }
        }

        public double ChieuRong
        {
            get { return mChieuRong; }
            set { mChieuRong = value; }
        }

        public HinhChuNhat()
        {
            mChieuDai = 1;
            mChieuRong = 1;
            tinhDienTichChuVi();
        }

        public HinhChuNhat(double dai, double rong)
        {
            ChieuDai = dai;
            ChieuRong = rong;
            tinhDienTichChuVi();
        }

        public override void tinhDienTichChuVi()
        {
            DienTich = mChieuDai * mChieuRong;
            ChuVi = (mChieuDai + mChieuRong) * 2;
        }

        public override string ToString()
        {
            return $"Hinh chu nhat - Dai: {ChieuDai}, Rong: {ChieuRong}, {base.ToString()}";
        }
    }

    class HinhTron : HinhHoc
    {
        private double mBanKinh;

        public double BanKinh
        {
            get { return mBanKinh; }
            set { mBanKinh = value; }
        }

        public HinhTron()
        {
            mBanKinh = 1;
            tinhDienTichChuVi();
        }

        public HinhTron(double banKinh)
        {
            BanKinh = banKinh;
            tinhDienTichChuVi();
        }

        public override void tinhDienTichChuVi()
        {
            DienTich = Math.PI * mBanKinh * mBanKinh;
            ChuVi = 2 * Math.PI * mBanKinh;
        }

        public override string ToString()
        {
            return $"Hinh tron - Ban kinh: {BanKinh}, {base.ToString()}";
        }
    }

    internal class Program
    {
        static List<HinhHoc> DocFile(string duongDan)
        {
            List<HinhHoc> ds = new List<HinhHoc>();

            string[] lines = File.ReadAllLines(duongDan);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(
                    new char[] { ' ', '\t' },
                    StringSplitOptions.RemoveEmptyEntries
                );

                string loai = parts[0].ToUpperInvariant();

                if (loai == "HCN" && parts.Length == 3)
                {
                    double dai = double.Parse(parts[1], CultureInfo.InvariantCulture);
                    double rong = double.Parse(parts[2], CultureInfo.InvariantCulture);

                    if (dai <= 0 || rong <= 0)
                        throw new FormatException("Kich thuoc phai lon hon 0.");

                    ds.Add(new HinhChuNhat(dai, rong));
                }
                else if (loai == "HT" && parts.Length == 2)
                {
                    double banKinh = double.Parse(parts[1], CultureInfo.InvariantCulture);

                    if (banKinh <= 0)
                        throw new FormatException("Ban kinh phai lon hon 0.");

                    ds.Add(new HinhTron(banKinh));
                }
                else
                {
                    throw new FormatException("Dong du lieu khong hop le: " + line);
                }
            }

            return ds;
        }

        static void XuatDanhSach(List<HinhHoc> ds)
        {
            foreach (HinhHoc hinh in ds)
            {
                hinh.tinhDienTichChuVi();
                Console.WriteLine(hinh);
            }
        }

        static void GhiFile(List<HinhHoc> ds, string duongDan)
        {
            using (StreamWriter sw = new StreamWriter(duongDan))
            {
                foreach (HinhHoc hinh in ds)
                {
                    hinh.tinhDienTichChuVi();
                    sw.WriteLine(hinh.ToString());
                }
            }
        }

        static void Main(string[] args)
        {
            HinhChuNhat hcn1 = new HinhChuNhat();
            hcn1.ChieuDai = 8;
            hcn1.ChieuRong = 4;
            hcn1.tinhDienTichChuVi();

            HinhChuNhat hcn2 = new HinhChuNhat(10, 5);

            HinhTron ht1 = new HinhTron();
            ht1.BanKinh = 3;
            ht1.tinhDienTichChuVi();

            HinhTron ht2 = new HinhTron(5);

            Console.WriteLine("KIEM TRA CAC LOP");
            Console.WriteLine(hcn1);
            Console.WriteLine(hcn2);
            Console.WriteLine(ht1);
            Console.WriteLine(ht2);

            string input = "input.txt";
            string output = "output.txt";

            try
            {
                List<HinhHoc> ds = DocFile(input);

                Console.WriteLine("\nDANH SACH HINH HOC TU FILE");
                XuatDanhSach(ds);

                GhiFile(ds, output);

                Console.WriteLine("\nDa ghi ket qua vao output.txt");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Khong tim thay file input.txt");
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Loi du lieu: " + ex.Message);
            }
            catch (IOException ex)
            {
                Console.WriteLine("Loi doc ghi file: " + ex.Message);
            }

            Console.ReadKey();
        }
    }
}
