using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace IdslTracker.Classes
{
    internal class SignatureClass
    {
        public static void UploadImage(string connectionString, byte[] signatureBytes, string ID)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("update TrackingData set SignatureCopy =@Signature,Signed='1' where id = '" + ID + "'", conn);
                cmd.Parameters.AddWithValue("@Signature", signatureBytes);
                cmd.ExecuteNonQuery();

                conn.Close();
              

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static byte[] DownloadImage(string connectionString, string ID)
        {
            try
            {
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("select SignatureCopy from TrackingData where id='" + ID + "' ", conn);
                byte[] barrImg = cmd.ExecuteScalar() as byte[];
                conn.Close();
                return barrImg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
