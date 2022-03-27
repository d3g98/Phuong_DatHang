using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ThaoPhuong.Models;

namespace ThaoPhuong.Utils
{
    public class SessionUtils
    {
        public static bool IsAdmin(HttpSessionStateBase ses)
        {
            DbEntities db = new DbEntities();
            DKHACHHANG khRow = ses[Contants.USER_SESSION_NAME] as DKHACHHANG;
            khRow = db.DKHACHHANGs.Where(x => x.ID == khRow.ID).FirstOrDefault();
            return khRow == null ? false : khRow.ISADMIN == 30;
        }
    }
}