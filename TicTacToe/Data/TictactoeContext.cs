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

    public virtual DbSet<RoundClub> RoundClubs { get; set; }

    public virtual DbSet<RoundMove> RoundMoves { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserConnection> UserConnections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Club>(entity =>
        {
            entity.ToTable("Club");

            entity.Property(e => e.ClubId).HasColumnName("club_id");
            entity.Property(e => e.ApiTeamId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("api_team_id");
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
            entity.Property(e => e.IsP1Winner).HasColumnName("is_p1_winner");
            entity.Property(e => e.P1UserId).HasColumnName("p1_user_id");
            entity.Property(e => e.P2UserId).HasColumnName("p2_user_id");

            entity.HasOne(d => d.P1User).WithMany(p => p.GameP1Users)
                .HasForeignKey(d => d.P1UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_User");

            entity.HasOne(d => d.P2User).WithMany(p => p.GameP2Users)
                .HasForeignKey(d => d.P2UserId)
                .HasConstraintName("FK_Game_Game");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("Player");

            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.ApiPlayerId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("api_player_id");
            entity.Property(e => e.Birthdate)
                .HasColumnType("date")
                .HasColumnName("birthdate");
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
            entity.Property(e => e.IsP1Turn).HasColumnName("is_p1_turn");
            entity.Property(e => e.IsP1Win).HasColumnName("is_p1_win");
            entity.Property(e => e.RoundNo).HasColumnName("round_no");

            entity.HasOne(d => d.Game).WithMany(p => p.Rounds)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rounds_Game");
        });

        modelBuilder.Entity<RoundClub>(entity =>
        {
            entity.HasKey(e => e.GameClubId).HasName("PK_Game_Club");

            entity.ToTable("Round_Club");

            entity.Property(e => e.GameClubId).HasColumnName("game_club_id");
            entity.Property(e => e.ClubId).HasColumnName("club_id");
            entity.Property(e => e.ColNo).HasColumnName("col_no");
            entity.Property(e => e.RoundId).HasColumnName("round_id");
            entity.Property(e => e.RowNo).HasColumnName("row_no");

            entity.HasOne(d => d.Club).WithMany(p => p.RoundClubs)
                .HasForeignKey(d => d.ClubId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Club_Club");


            entity.HasOne(d => d.Round).WithMany(p => p.RoundClubs)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Club_Game")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RoundMove>(entity =>
        {
            entity.HasKey(e => e.MoveId).HasName("PK_Game_Moves");

            entity.ToTable("Round_Moves");

            entity.Property(e => e.MoveId).HasColumnName("move_id");
            entity.Property(e => e.CellValue)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("cellValue");
            entity.Property(e => e.ColNo).HasColumnName("colNo");
            entity.Property(e => e.RoundId).HasColumnName("round_id");
            entity.Property(e => e.RowNo).HasColumnName("rowNo");

            entity.HasOne(d => d.Round).WithMany(p => p.RoundMoves)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Moves_Game")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<UserConnection>(entity =>
        {
            entity.ToTable("User_Connection");

            entity.Property(e => e.UserConnectionId).HasColumnName("user_connection_id");
            entity.Property(e => e.ConnectionId)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("connection_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
