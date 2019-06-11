/*
    OrderMaker - http://ordermaker.org
    Copyright(c) 2019 Oleg Bruev. All rights reserved.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see https://www.gnu.org/licenses/.
*/

using Microsoft.EntityFrameworkCore;

namespace Mtd.OrderMaker.Web.Data
{
    public partial class OrderMakerContext : DbContext
    {
        public OrderMakerContext()
        {
        }

        public OrderMakerContext(DbContextOptions<OrderMakerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MtdConfigFile> MtdConfigFiles { get; set; }
        public virtual DbSet<MtdFilter> MtdFilter { get; set; }
        public virtual DbSet<MtdFilterColumn> MtdFilterColumn { get; set; }
        public virtual DbSet<MtdFilterField> MtdFilterField { get; set; }
        public virtual DbSet<MtdForm> MtdForm { get; set; }
        public virtual DbSet<MtdFormDesk> MtdFormDesk { get; set; }
        public virtual DbSet<MtdFormHeader> MtdFormHeader { get; set; }
        public virtual DbSet<MtdFormList> MtdFormList { get; set; }
        public virtual DbSet<MtdFormPart> MtdFormPart { get; set; }
        public virtual DbSet<MtdFormPartField> MtdFormPartField { get; set; }
        public virtual DbSet<MtdFormPartHeader> MtdFormPartHeader { get; set; }
        public virtual DbSet<MtdGroupForm> MtdGroupForm { get; set; }
        public virtual DbSet<MtdLogDocument> MtdLogDocument { get; set; }
        public virtual DbSet<MtdStore> MtdStore { get; set; }
        public virtual DbSet<MtdStoreLink> MtdStoreLink { get; set; }
        public virtual DbSet<MtdStoreStack> MtdStoreStack { get; set; }
        public virtual DbSet<MtdStoreStackDate> MtdStoreStackDate { get; set; }
        public virtual DbSet<MtdStoreStackDecimal> MtdStoreStackDecimal { get; set; }
        public virtual DbSet<MtdStoreStackFile> MtdStoreStackFile { get; set; }
        public virtual DbSet<MtdStoreStackInt> MtdStoreStackInt { get; set; }
        public virtual DbSet<MtdStoreStackText> MtdStoreStackText { get; set; }
        public virtual DbSet<MtdSysStyle> MtdSysStyle { get; set; }
        public virtual DbSet<MtdSysTerm> MtdSysTerm { get; set; }
        public virtual DbSet<MtdSysType> MtdSysType { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<MtdConfigFile>(entity =>
            {
                entity.ToTable("mtd_config_file");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FileData)
                    .IsRequired()
                    .HasColumnName("file_data")
                    .HasColumnType("mediumblob");

                entity.Property(e => e.FileSize)
                    .IsRequired()
                    .HasColumnName("file_size")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.FileType)
                    .IsRequired()
                    .HasColumnName("file_type")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(45)");
            });

            modelBuilder.Entity<MtdFilter>(entity =>
            {
                entity.ToTable("mtd_filter");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.IdUser)
                    .HasName("IX_INDEX_USER");

                entity.HasIndex(e => e.MtdForm)
                    .HasName("mtd_filter_mtd_form_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IdUser)
                    .IsRequired()
                    .HasColumnName("idUser")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.MtdForm)
                    .IsRequired()
                    .HasColumnName("mtd_form")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Page)
                    .HasColumnName("page")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.PageSize)
                    .HasColumnName("page_size")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'10'");

                entity.Property(e => e.SearchNumber)
                    .IsRequired()
                    .HasColumnName("searchNumber")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.SearchText)
                    .IsRequired()
                    .HasColumnName("searchText")
                    .HasColumnType("varchar(256)");

                entity.HasOne(d => d.MtdFormNavigation)
                    .WithMany(p => p.MtdFilter)
                    .HasForeignKey(d => d.MtdForm)
                    .HasConstraintName("mtd_filter_mtd_form");
            });

            modelBuilder.Entity<MtdFilterColumn>(entity =>
            {
                entity.ToTable("mtd_filter_column");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdFilter)
                    .HasName("mtd_filter_column_idx");

                entity.HasIndex(e => e.MtdFormPartField)
                    .HasName("mtd_roster_field_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MtdFilter)
                    .HasColumnName("mtd_filter")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MtdFormPartField)
                    .IsRequired()
                    .HasColumnName("mtd_form_part_field")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Sequence)
                    .HasColumnName("sequence")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.MtdFilterNavigation)
                    .WithMany(p => p.MtdFilterColumn)
                    .HasForeignKey(d => d.MtdFilter)
                    .HasConstraintName("mtd_filter_column_mtd_field");

                entity.HasOne(d => d.MtdFormPartFieldNavigation)
                    .WithMany(p => p.MtdFilterColumn)
                    .HasForeignKey(d => d.MtdFormPartField)
                    .HasConstraintName("mtd_roster_field");
            });

            modelBuilder.Entity<MtdFilterField>(entity =>
            {
                entity.ToTable("mtd_filter_field");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdFilter)
                    .HasName("mtd_filter_idx");

                entity.HasIndex(e => e.MtdFormPartField)
                    .HasName("mtd_filter_field_mtd_form_field_idx");

                entity.HasIndex(e => e.MtdTerm)
                    .HasName("mtd_filter_field_term_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.MtdFilter)
                    .HasColumnName("mtd_filter")
                    .HasColumnType("int(11)");

                entity.Property(e => e.MtdFormPartField)
                    .IsRequired()
                    .HasColumnName("mtd_form_part_field")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.MtdTerm)
                    .HasColumnName("mtd_term")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasColumnType("varchar(256)");

                entity.HasOne(d => d.MtdFilterNavigation)
                    .WithMany(p => p.MtdFilterField)
                    .HasForeignKey(d => d.MtdFilter)
                    .HasConstraintName("mtd_filter_field_mtd_field");

                entity.HasOne(d => d.MtdFormPartFieldNavigation)
                    .WithMany(p => p.MtdFilterField)
                    .HasForeignKey(d => d.MtdFormPartField)
                    .HasConstraintName("mtd_filter_field_mtd_form_field");

                entity.HasOne(d => d.MtdTermNavigation)
                    .WithMany(p => p.MtdFilterField)
                    .HasForeignKey(d => d.MtdTerm)
                    .HasConstraintName("mtd_filter_field_mtd_term");
            });

            modelBuilder.Entity<MtdForm>(entity =>
            {
                entity.ToTable("mtd_form");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdGroup)
                    .HasName("fk_form_grooup_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.MtdGroup)
                    .IsRequired()
                    .HasColumnName("mtd_group")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(120)");

                entity.Property(e => e.VisibleNumber)
                    .IsRequired()
                    .HasColumnName("visible_number")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.VisibleDate)
                    .IsRequired()
                    .HasColumnName("visible_date")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Sequence)
                    .HasColumnName("sequence")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.MtdGroupNavigation)
                    .WithMany(p => p.MtdForm)
                    .HasForeignKey(d => d.MtdGroup)
                    .HasConstraintName("fk_form_grooup");

                entity.HasOne(d => d.ParentNavigation)
                    .WithMany(p => p.InverseParentNavigation)
                    .HasForeignKey(d => d.Parent)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_form_parent");

            });

            modelBuilder.Entity<MtdFormDesk>(entity =>
            {
                entity.ToTable("mtd_form_desk");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.ColorBack)
                    .IsRequired()
                    .HasColumnName("color_back")
                    .HasColumnType("varchar(45)")
                    .HasDefaultValueSql("'gray'");

                entity.Property(e => e.ColorFont)
                    .IsRequired()
                    .HasColumnName("color_font")
                    .HasColumnType("varchar(45)")
                    .HasDefaultValueSql("'white'");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasColumnName("image")
                    .HasColumnType("mediumblob");

                entity.Property(e => e.ImageSize)
                    .HasColumnName("image_size")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ImageType)
                    .IsRequired()
                    .HasColumnName("image_type")
                    .HasColumnType("varchar(256)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdFormDesk)
                    .HasForeignKey<MtdFormDesk>(d => d.Id)
                    .HasConstraintName("fk_mtd_form_des_mtd_from");
            });

            modelBuilder.Entity<MtdFormHeader>(entity =>
            {
                entity.ToTable("mtd_form_header");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasColumnName("image")
                    .HasColumnType("mediumblob");

                entity.Property(e => e.ImageSize)
                    .HasColumnName("image_size")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ImageType)
                    .IsRequired()
                    .HasColumnName("image_type")
                    .HasColumnType("varchar(256)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdFormHeader)
                    .HasForeignKey<MtdFormHeader>(d => d.Id)
                    .HasConstraintName("fk_image_form");
            });

            modelBuilder.Entity<MtdFormList>(entity =>
            {
                entity.ToTable("mtd_form_list");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdForm)
                    .HasName("fk_list_form_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.MtdForm)
                    .IsRequired()
                    .HasColumnName("mtd_form")
                    .HasColumnType("varchar(36)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdFormList)
                    .HasForeignKey<MtdFormList>(d => d.Id)
                    .HasConstraintName("fk_list_field");

                entity.HasOne(d => d.MtdFormNavigation)
                    .WithMany(p => p.MtdFormList)
                    .HasForeignKey(d => d.MtdForm)
                    .HasConstraintName("fk_list_form");
            });

            modelBuilder.Entity<MtdFormPart>(entity =>
            {
                entity.ToTable("mtd_form_part");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdForm)
                    .HasName("fk_mtd_form_part_mtd_form1_idx");

                entity.HasIndex(e => e.MtdSysStyle)
                    .HasName("fk_mtd_form_part_mtd_sys_style1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.MtdForm)
                    .IsRequired()
                    .HasColumnName("mtd_form")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.MtdSysStyle)
                    .HasColumnName("mtd_sys_style")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(120)");

                entity.Property(e => e.Sequence)
                    .HasColumnName("sequence")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Child)
                    .HasColumnName("child")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.MtdFormNavigation)
                    .WithMany(p => p.MtdFormPart)
                    .HasForeignKey(d => d.MtdForm)
                    .HasConstraintName("fk_mtd_form_part_mtd_form1");

                entity.HasOne(d => d.MtdSysStyleNavigation)
                    .WithMany(p => p.MtdFormPart)
                    .HasForeignKey(d => d.MtdSysStyle)
                    .HasConstraintName("fk_mtd_form_part_mtd_sys_style1");
            });

            modelBuilder.Entity<MtdFormPartField>(entity =>
            {
                entity.ToTable("mtd_form_part_field");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdFormPart)
                    .HasName("fk_mtd_form_part_field_mtd_form_part1_idx");

                entity.HasIndex(e => e.MtdSysType)
                    .HasName("fk_mtd_form_part_field_mtd_sys_type1_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.MtdFormPart)
                    .IsRequired()
                    .HasColumnName("mtd_form_part")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.MtdSysType)
                    .HasColumnName("mtd_sys_type")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(120)");

                entity.Property(e => e.Required)
                    .HasColumnName("required")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Sequence)
                    .HasColumnName("sequence")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.MtdFormPartNavigation)
                    .WithMany(p => p.MtdFormPartField)
                    .HasForeignKey(d => d.MtdFormPart)
                    .HasConstraintName("fk_mtd_form_part_field_mtd_form_part1");

                entity.HasOne(d => d.MtdSysTypeNavigation)
                    .WithMany(p => p.MtdFormPartField)
                    .HasForeignKey(d => d.MtdSysType)
                    .HasConstraintName("fk_mtd_form_part_field_mtd_sys_type1");
            });

            modelBuilder.Entity<MtdFormPartHeader>(entity =>
            {
                entity.ToTable("mtd_form_part_header");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasColumnName("image")
                    .HasColumnType("mediumblob");

                entity.Property(e => e.ImageSize)
                    .HasColumnName("image_size")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ImageType)
                    .IsRequired()
                    .HasColumnName("image_type")
                    .HasColumnType("varchar(256)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdFormPartHeader)
                    .HasForeignKey<MtdFormPartHeader>(d => d.Id)
                    .HasConstraintName("fk_image_form_part");
            });

            modelBuilder.Entity<MtdGroupForm>(entity =>
            {
                entity.ToTable("mtd_group_form");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Parent)
                    .HasName("fk_group_themself_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(120)");

                entity.Property(e => e.Parent)
                    .IsRequired()
                    .HasColumnName("parent")
                    .HasColumnType("varchar(36)");
            });

            modelBuilder.Entity<MtdLogDocument>(entity =>
            {
                entity.ToTable("mtd_log_document");

                entity.HasIndex(e => e.TimeCh)
                    .HasName("ix_date");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdStore)
                    .HasName("fk_log_document_store_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TimeCh)
                    .HasColumnName("timech")
                    .HasColumnType("datetime");

                entity.Property(e => e.MtdStore)
                    .IsRequired()
                    .HasColumnName("mtd_store")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("varchar(36)");

                entity.HasOne(d => d.MtdStoreNavigation)
                    .WithMany(p => p.MtdLogDocument)
                    .HasForeignKey(d => d.MtdStore)
                    .HasConstraintName("fk_log_document_store");
            });

            modelBuilder.Entity<MtdStore>(entity =>
            {
                entity.ToTable("mtd_store");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdForm)
                    .HasName("fk_mtd_store_mtd_form1_idx");

                entity.HasIndex(e => e.Timecr)
                    .HasName("IX_TIMECR");

                entity.HasIndex(e => new { e.MtdForm, e.Sequence })
                    .HasName("Seq_Unique")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.MtdForm)
                    .IsRequired()
                    .HasColumnName("mtd_form")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Sequence)
                    .HasColumnName("sequence")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Timecr)
                    .HasColumnName("timecr")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'CURRENT_TIMESTAMP'");

                entity.HasOne(d => d.MtdFormNavigation)
                    .WithMany(p => p.MtdStore)
                    .HasForeignKey(d => d.MtdForm)
                    .HasConstraintName("fk_mtd_store_mtd_form1");

                entity.HasOne(d => d.ParentNavigation)
                    .WithMany(p => p.InverseParentNavigation)
                    .HasForeignKey(d => d.Parent)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_mtd_store_parent");
            });

            modelBuilder.Entity<MtdStoreLink>(entity =>
            {
                entity.ToTable("mtd_store_link");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdStore)
                    .HasName("fk_mtd_store_link_mtd_store1_idx");

                entity.HasIndex(e => new { e.MtdStore, e.Id })
                    .HasName("ix_unique")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.MtdStore)
                    .IsRequired()
                    .HasColumnName("mtd_store")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.Register)
                    .IsRequired()
                    .HasColumnType("varchar(768)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreLink)
                    .HasForeignKey<MtdStoreLink>(d => d.Id)
                    .HasConstraintName("fk_mtd_store_link_mtd_store_stack");

                entity.HasOne(d => d.MtdStoreNavigation)
                    .WithMany(p => p.MtdStoreLink)
                    .HasForeignKey(d => d.MtdStore)
                    .HasConstraintName("fk_mtd_store_link_mtd_store1");
            });

            modelBuilder.Entity<MtdStoreStack>(entity =>
            {
                entity.ToTable("mtd_store_stack");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.MtdFormPartField)
                    .HasName("fk_mtd_store_stack_mtd_form_part_field1_idx");

                entity.HasIndex(e => e.MtdStore)
                    .HasName("fk_mtd_store_stack_mtd_store_idx");

                entity.HasIndex(e => new { e.MtdStore, e.MtdFormPartField })
                    .HasName("IX_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.MtdFormPartField)
                    .IsRequired()
                    .HasColumnName("mtd_form_part_field")
                    .HasColumnType("varchar(36)");

                entity.Property(e => e.MtdStore)
                    .IsRequired()
                    .HasColumnName("mtd_store")
                    .HasColumnType("varchar(36)");

                entity.HasOne(d => d.MtdFormPartFieldNavigation)
                    .WithMany(p => p.MtdStoreStack)
                    .HasForeignKey(d => d.MtdFormPartField)
                    .HasConstraintName("fk_mtd_store_stack_mtd_form_part_field1");

                entity.HasOne(d => d.MtdStoreNavigation)
                    .WithMany(p => p.MtdStoreStack)
                    .HasForeignKey(d => d.MtdStore)
                    .HasConstraintName("fk_mtd_store_stack_mtd_store");
            });

            modelBuilder.Entity<MtdStoreStackDate>(entity =>
            {
                entity.ToTable("mtd_store_stack_date");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Register)
                    .HasName("IX_DATESTACK");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Register)
                    .HasColumnName("register")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreStackDate)
                    .HasForeignKey<MtdStoreStackDate>(d => d.Id)
                    .HasConstraintName("fk_date_stack");
            });

            modelBuilder.Entity<MtdStoreStackDecimal>(entity =>
            {
                entity.ToTable("mtd_store_stack_decimal");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Register)
                    .HasName("IX_DECIMALREGISTER");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Register)
                    .HasColumnName("register")
                    .HasColumnType("decimal(20,2)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreStackDecimal)
                    .HasForeignKey<MtdStoreStackDecimal>(d => d.Id)
                    .HasConstraintName("fk_decimal_stack");
            });

            modelBuilder.Entity<MtdStoreStackFile>(entity =>
            {
                entity.ToTable("mtd_store_stack_file");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasColumnName("file_name")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.FileSize)
                    .HasColumnName("file_size")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FileType)
                    .IsRequired()
                    .HasColumnName("file_type")
                    .HasColumnType("varchar(256)");

                entity.Property(e => e.Register)
                    .IsRequired()
                    .HasColumnName("register")
                    .HasColumnType("mediumblob");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreStackFile)
                    .HasForeignKey<MtdStoreStackFile>(d => d.Id)
                    .HasConstraintName("fk_file_stack");
            });

            modelBuilder.Entity<MtdStoreStackInt>(entity =>
            {
                entity.ToTable("mtd_store_stack_int");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Register)
                    .HasName("IX_INTSTACK");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Register)
                    .HasColumnName("register")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreStackInt)
                    .HasForeignKey<MtdStoreStackInt>(d => d.Id)
                    .HasConstraintName("fk_int_stack");
            });

            modelBuilder.Entity<MtdStoreStackText>(entity =>
            {
                entity.ToTable("mtd_store_stack_text");

                entity.HasIndex(e => e.Id)
                    .HasName("category_id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Register)
                    .HasName("IX_REGISTER_TEXT");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Register)
                    .IsRequired()
                    .HasColumnName("register")
                    .HasColumnType("varchar(768)");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.MtdStoreStackText)
                    .HasForeignKey<MtdStoreStackText>(d => d.Id)
                    .HasConstraintName("fk_text_stack");
            });

            modelBuilder.Entity<MtdSysStyle>(entity =>
            {
                entity.ToTable("mtd_sys_style");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(120)");
            });

            modelBuilder.Entity<MtdSysTerm>(entity =>
            {
                entity.ToTable("mtd_sys_term");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(45)");

                entity.Property(e => e.Sign)
                    .IsRequired()
                    .HasColumnName("sign")
                    .HasColumnType("varchar(45)");
            });

            modelBuilder.Entity<MtdSysType>(entity =>
            {
                entity.ToTable("mtd_sys_type");

                entity.HasIndex(e => e.Id)
                    .HasName("id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Active)
                    .HasColumnName("active")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(512)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(120)");
            });
        }
    }
}
