using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TicTacToe.Data;
using TicTacToe.Models;

namespace TicTacToe.Controllers
{
    public class HomeController : Controller
    {
        private readonly TictactoeContext _context;
        private static Random rnd= new Random();
        public HomeController(TictactoeContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult CreateGame(string playerName)
        {
            Game game = Game.createGame(playerName);
            _context.Games.Add(game);
            _context.SaveChanges();
            return View();
        }

        public ActionResult ConnectGame(string playerName, string gameCode)
        {
            if (_context.Games.Where(g => g.GameCode == gameCode).ToList().Count > 0)
            {
                Game game = _context.Games.Where(g => g.GameCode == gameCode).First();
                if (!game.IsBeingPlayed && !game.IsFinished)
                {
                    game.P2Name = playerName;
                    GameClub gameClub;
                    List<Club> clubs = _context.Clubs.ToList();
                    List<Club> selectedClubs = new List<Club>();
                    List<Club> possibleClubsForVerticalAlign = new List<Club>();
                    int r;
                    for (int i = 0; i <= 3; i++)
                    {
                        if (i == 0)
                        {
                            for (int j = 1; j <= 3; j++)
                            {
                                r = rnd.Next(clubs.Count() - selectedClubs.Count() - 1);
                                Club club = clubs.Except(selectedClubs).ToList()[r];
                                selectedClubs.Add(club);
                                gameClub = new GameClub();
                                gameClub.ClubId = club.ClubId;
                                gameClub.ColNo = j;
                                gameClub.RowNo = i;
                                game.GameClubs.Add(gameClub);
                            }
                            string club_ids = String.Join(",", game.GameClubs.Where(p => p.RowNo == 0).Select(c => c.ClubId).ToList());
                            possibleClubsForVerticalAlign = _context.Clubs.FromSqlInterpolated($"Generate_Possible_Clubs {club_ids}").ToList();
                        }
                        else
                        {

                            List<Club> possibleClubThatCanBeSelected = possibleClubsForVerticalAlign.Except(selectedClubs).ToList();
                            r = rnd.Next(possibleClubThatCanBeSelected.Count() - 1);
                            selectedClubs.Add(possibleClubThatCanBeSelected[r]);
                            gameClub = new GameClub();
                            gameClub.ClubId = possibleClubThatCanBeSelected[r].ClubId;
                            gameClub.ColNo = 0;
                            gameClub.RowNo = i;
                            game.GameClubs.Add(gameClub);
                        }
                    }
                    game.Rounds.Add(new Round { IsFinished = false, IsP1Win = false, RoundNo = 1, });
                    _context.SaveChanges();
                    return View();
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult MakeMove(GamePlay move)
        {
            if(_context.Games.Where(g=>g.GameId==move.GameId).Count()>0)
            {
                Game thisGame = _context.Games.Where(g => g.GameId == move.GameId).First();
                if (!thisGame.IsFinished)
                {
                    List<GameMove> gameMoves = _context.Games.First().GameMoves.ToList();
                    gameMoves.Add(new GameMove { GameId=move.GameId,ColNo=move.CoordinateY,RowNo=move.CoordinateX,CellValue=(int)move.Movetype });

                    bool isFirstPlayerWinner = CheckIfThereIsAnyWinner(gameMoves,TicTacToeTypes.X);
                    bool isSecondPlayerWinner = CheckIfThereIsAnyWinner(gameMoves, TicTacToeTypes.Y);

                    Round actualRound = thisGame.Rounds.OrderByDescending(r => r.RoundNo).First();
                    if (isFirstPlayerWinner || isSecondPlayerWinner)
                        actualRound.IsFinished = true;  
                    if (isFirstPlayerWinner)
                        actualRound.IsP1Win = true;
                    if (isSecondPlayerWinner)
                        actualRound.IsP1Win = false;
                    
                    if(thisGame.Rounds.Where(r=>(bool)r.IsP1Win).Count()==3 || thisGame.Rounds.Where(r => (bool)!r.IsP1Win).Count() == 3)
                    {
                        thisGame.IsP1Winner = thisGame.Rounds.Where(r => (bool)r.IsP1Win).Count() == 3;
                        thisGame.IsBeingPlayed = false;
                        thisGame.IsFinished = true;
                    }
                    else if((bool)actualRound.IsFinished)
                    {
                        thisGame.Rounds.Add(new Round { IsFinished = false, IsP1Win = false, RoundNo = actualRound.RoundNo++ });
                    }
                    _context.SaveChanges();
                    return View();
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        public bool CheckIfThereIsAnyWinner(List<GameMove> gameMoves,TicTacToeTypes player)
        {
            int[] boardArray = new int[9];
            int counterIndex = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    boardArray[counterIndex] = gameMoves.Where(gm=>gm.RowNo==i && gm.ColNo==j).First().CellValue;
                    counterIndex++;
                }
            }

            int[,] winCombinations =  new int[8, 3] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };
            for (int i = 0; i < winCombinations.Length; i++)
            {
                int counter = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (boardArray[winCombinations[i,j]] == (int)player)
                    {
                        counter++;
                        if (counter == 3)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }  
}
