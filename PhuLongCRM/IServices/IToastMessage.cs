﻿using System;
namespace PhuLongCRM.IServices
{
    public interface IToastMessage
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
