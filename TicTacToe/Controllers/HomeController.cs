using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
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
        public IActionResult Game()
        {
            return PartialView();
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
            User newUser = new User();
            newUser.UserName = playerName;
            _context.Users.Add(newUser);
            _context.SaveChanges();
            Game game = Models.Game.createGame();
            game.P1UserId = _context.Users.Where(p => p.UserName == playerName).OrderByDescending(p=>p.UserId).Last().UserId;
            _context.Games.Add(game);
            _context.SaveChanges();
            return PartialView("WaitingRoom");
        }

        public ActionResult JoinRoom(string playerName)
        {
            User newUser = new User();
            newUser.UserName = playerName;
            _context.Users.Add(newUser);
            _context.SaveChanges();
            ViewData.Model= _context.Users.Where(p => p.UserName == playerName).OrderByDescending(p => p.UserId).Last().UserId;
            return PartialView();
        }

        public ActionResult ConnGame(string gameCode)
        {
            Game game = _context.Games.Where(p => p.GameCode == gameCode).Include(g=>g.Rounds).Include(g=>g.GameClubs).First();
            return PartialView("Game", game);
        }
        public ActionResult ConnectGame(int playerId, string gameCode)
        {
            if (_context.Games.Where(g => g.GameCode == gameCode).ToList().Count > 0)
            {
                Game game = _context.Games.Where(g => g.GameCode == gameCode).First();
                if (!game.IsBeingPlayed && !game.IsFinished)
                {
                    game.P2UserId = playerId;
                    game.IsBeingPlayed = true;
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
                    game.Rounds.Add(new Round { IsFinished = false, IsP1Win = false, RoundNo = 1 });
                    _context.SaveChanges();
                    return PartialView("Game", game);
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

        [HttpGet]
        public ActionResult MakeMove(int GameId, int CoordinateX, int CoordinateY,int PlayerId,TicTacToeTypes Movetype)
        {
            if(_context.Games.Where(g=>g.GameId==GameId && g.IsBeingPlayed).Count()>0)
            {
                Game thisGame = _context.Games.Where(g => g.GameId == GameId).Include(p => p.Rounds).ThenInclude(p=>p.GameMoves).Include(g=>g.GameClubs).First();
                if (!thisGame.IsFinished)
                {
                    Round actualRound = thisGame.Rounds.Where(r=>!(bool)r.IsFinished).OrderByDescending(r => r.RoundNo).First();
                    List<GameClub> gameClubsForTheMove =
                        thisGame.GameClubs.Where(gc => gc.ColNo == CoordinateY+1 || gc.RowNo == CoordinateX+1).ToList();

                    bool hasPlayerPlayedForFirstClub = _context.PlayerClubHistories.Where(p => p.PlayerId == PlayerId && p.ClubId == gameClubsForTheMove[0].ClubId).Count() > 0;
                    bool hasPlayerPlayedForSecondClub = _context.PlayerClubHistories.Where(p => p.PlayerId == PlayerId && p.ClubId == gameClubsForTheMove[1].ClubId).Count() > 0;
                    if (hasPlayerPlayedForFirstClub && hasPlayerPlayedForSecondClub)
                        actualRound.GameMoves.Add(new GameMove { ColNo = CoordinateY, RowNo = CoordinateX, CellValue = (int)Movetype });
                    else
                        return View();

                    bool isFirstPlayerWinner = CheckIfThereIsAnyWinner(actualRound.GameMoves.ToList(), TicTacToeTypes.X);
                    bool isSecondPlayerWinner = CheckIfThereIsAnyWinner(actualRound.GameMoves.ToList(), TicTacToeTypes.Y);

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
                        int roundNo = actualRound.RoundNo+1;
                        thisGame.Rounds.Add(new Round { IsFinished = false, IsP1Win = false, RoundNo = roundNo });
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
                    List<GameMove> moves = gameMoves.Where(gm => gm.RowNo == i && gm.ColNo == j).ToList();
                    boardArray[counterIndex] = moves.Count()>0 ? moves.First().CellValue : 2;
                    counterIndex++;
                }
            }

            int[,] winCombinations =  new int[8, 3] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };
            for (int i = 0; i < 8; i++)
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

        [HttpGet]
        public List<Player> GetPlayersAutoComplete(string playerName)
        {
            var players = from m in _context.Players
                          where m.PlayerName.Contains(playerName)
                          select m;
            return players.ToList();
        }
    }  
}
