﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Entity;

namespace Uoko.FireProj.DataAccess.FluentAPI
{
    public class DictionaryMap : EntityTypeConfiguration<Dictionary>
    {
        public DictionaryMap()
        {
            Property(r => r.Name).HasMaxLength(50).IsRequired();//长度50,必填
            Property(r => r.Value).IsRequired();
            ToTable("Dictionary");//指定生成表名
        }
    }
}
