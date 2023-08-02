using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Models;

namespace TicTacToe.Data;

public partial class TictactoeContext : DbContext
{
    public TictactoeContext()
    {
    }

    public TictactoeContext(DbContextOptions<TictactoeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Club> Clubs { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerClubHistory> PlayerClubHistories { get; set; }

    public virtual DbSet<Round> Rounds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=TICTACTOE;Integrated Security = true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Club>(entity =>
        {
            entity.ToTable("Club");

            entity.Property(e => e.ClubId).HasColumnName("club_id");
            entity.Property(e => e.ClubLogo)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("club_logo");
            entity.Property(e => e.ClubName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("club_name");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("Game");

            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.GameCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("game_code");
            entity.Property(e => e.IsBeingPlayed).HasColumnName("is_being_played");
            entity.Property(e => e.IsFinished).HasColumnName("is_finished");
            entity.Property(e => e.P1Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("p1_name");
            entity.Property(e => e.P2Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("p2_name");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("Player");

            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.PlayerName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("player_name");
        });

        modelBuilder.Entity<PlayerClubHistory>(entity =>
        {
            entity.ToTable("Player_Club_History");

            entity.Property(e => e.PlayerClubHistoryId).HasColumnName("player_club_history_id");
            entity.Property(e => e.ClubId).HasColumnName("club_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");

            entity.HasOne(d => d.Club).WithMany(p => p.PlayerClubHistories)
                .HasForeignKey(d => d.ClubId)
                .HasConstraintName("FK_Player_Club_History_Club");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerClubHistories)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("FK_Player_Club_History_Player_Club_History");
        });

        modelBuilder.Entity<Round>(entity =>
        {
            entity.Property(e => e.RoundId).HasColumnName("round_id");
            entity.Property(e => e.GameId).HasColumnName("game_id");
            entity.Property(e => e.IsFinished).HasColumnName("is_finished");
            entity.Property(e => e.IsP1Win).HasColumnName("is_p1_win");
            entity.Property(e => e.RoundNo).HasColumnName("round_no");

            entity.HasOne(d => d.Game).WithMany(p => p.Rounds)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rounds_Game");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
