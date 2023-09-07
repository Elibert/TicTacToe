using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TicTacToe.Data;
using TicTacToe.Helpers;
using TicTacToe.Models;
using TicTacToe.Signal;

namespace TicTacToe.Controllers
{
    public class HomeController : Controller
    {
        private readonly TictactoeContext _context;
        private static Random rnd= new Random();
        private SignalRSender signal;
        private readonly FunctionHelper functionHelper;

        public HomeController(TictactoeContext context,FunctionHelper _functionHelper, SignalRSender signalRSender)
        {
            _context = context;
            signal = signalRSender;
            functionHelper = _functionHelper;
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
            User newUser = new User();
            newUser.UserName = playerName;
            _context.Users.Add(newUser);
            _context.SaveChanges();
            SetCookie(playerName);
            Game game = Models.Game.createGame();
            game.GameCode = functionHelper.GenerateCode(7);
            game.P1UserId = _context.Users.Where(p => p.UserName == playerName).OrderByDescending(p=>p.UserId).Last().UserId;
            _context.Games.Add(game);
            _context.SaveChanges();
            return PartialView("WaitingRoom",game.GameCode);
        }
        
        public ActionResult JoinRoom(string playerName)
        {
            User newUser = new User();
            newUser.UserName = playerName;
            _context.Users.Add(newUser);
            _context.SaveChanges();
            SetCookie(playerName);
            ViewData.Model= _context.Users.Where(p => p.UserName == playerName).OrderByDescending(p => p.UserId).Last().UserId;
            return PartialView();
        }

        public ActionResult ConnGame(int gameId)
        {
            Game game = _context.Games.Where(p => p.GameId == gameId).Include(g=>g.Rounds)
                                                                     .ThenInclude(r=>r.RoundClubs)
                                                                     .ThenInclude(rc => rc.Club)
                                                                     .Include(g=>g.P1User)
                                                                     .Include(g=>g.P2User).First();
            game.OpponentUserId = (int)game.P2UserId;
            game.CurrentRound.isPlayerTurn = true;
            game.MoveType = TicTacToeTypes.X;
            return PartialView("Game", game);
        }

        public ActionResult Game(string gamecode)
        {
            Game game = _context.Games.Where(p => p.GameCode == gamecode).Include(g => g.Rounds)
                                                         .ThenInclude(r => r.RoundClubs)
                                                         .ThenInclude(rc => rc.Club)
                                                         .Include(r=>r.Rounds)
                                                         .ThenInclude(rm=>rm.RoundMoves)
                                                         .Include(g => g.P1User)
                                                         .Include(g => g.P2User).First();
            string[] userInfo;
            bool isP1player;
            if (!string.IsNullOrWhiteSpace(Request.Cookies["UC"]))
            {
                string decryptedCookieValue = FunctionHelper.EncryptDecryptValue(false, Request.Cookies["UC"]);
                userInfo = decryptedCookieValue.Split("_");
                if (userInfo.Length == 2)
                {
                    if (game.P1UserId == Convert.ToInt32(userInfo[1]))
                    {
                        game.OpponentUserId = (int)game.P2UserId;
                        game.MoveType = TicTacToeTypes.X;
                        isP1player = true;
                    }
                    else if (game.P2UserId == Convert.ToInt32(userInfo[1]))
                    {
                        game.OpponentUserId = game.P1UserId;
                        game.MoveType = TicTacToeTypes.O;
                        isP1player = false;
                    }
                    else
                    {
                        return View("Game", null);
                    }
                }
                else
                {
                    return View("Game", null);
                }

                if ((isP1player && (bool)game.CurrentRound.IsP1Turn) || (!isP1player && (bool)!game.CurrentRound.IsP1Turn))
                    game.CurrentRound.isPlayerTurn = true;
                else if ((isP1player && (bool)!game.CurrentRound.IsP1Turn) || (!isP1player && (bool)game.CurrentRound.IsP1Turn))
                    game.CurrentRound.isPlayerTurn = false;
                return View("Game", game);
            }
            else
            {
                return View("Game", null);
            }
        }

        public ActionResult ConnectGame(int playerId, string gameCode)
        {
            if (_context.Games.Where(g => g.GameCode == gameCode).ToList().Count > 0)
            {
                Game game = _context.Games.Where(g => g.GameCode == gameCode).First();
                if (!game.IsBeingPlayed && !game.IsFinished)
                {
                    game.P2UserId = playerId;
                    game.OpponentUserId = game.P1UserId;
                    game.IsBeingPlayed = true;
                    game.Rounds.Add(new Round { IsFinished = false, IsP1Win = false, RoundNo = 1, IsP1Turn=true });
                    Round currentRound = game.CurrentRound;
                    foreach (RoundClub selectedClub in SelectClubsForRound())
                        currentRound.RoundClubs.Add(selectedClub);

                    game.MoveType = TicTacToeTypes.O;
                    game.CurrentRound.isPlayerTurn = false;
                    _context.SaveChanges();
                    game = _context.Games.Where(g => g.GameCode == gameCode).Include(g => g.P1User)
                                                                            .Include(g => g.P2User)                                                                                     
                                                                            .Include(g => g.Rounds)
                                                                            .ThenInclude(r => r.RoundClubs)
                                                                            .ThenInclude(rc => rc.Club).First();
                    signal.EnterGame(_context.Games.Where(g => g.GameCode == gameCode).First().GameId);
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

        public void changeRoundClubs(int gameId)
        {
            Dictionary<string, string> newRoundClubs = new();
            Game game = _context.Games.Where(g => g.GameId == gameId)
                                    .Include(g => g.Rounds)
                                    .ThenInclude(r => r.RoundClubs)
                                    .Include(g=>g.Rounds)
                                    .ThenInclude(r=>r.RoundMoves)
                                    .First();

            game.Rounds.OrderByDescending(r=>r.RoundNo).First().RoundClubs.Clear();
            game.Rounds.OrderByDescending(r => r.RoundNo).First().RoundMoves.Clear();

            foreach (RoundClub club in SelectClubsForRound())
                game.Rounds.OrderByDescending(r => r.RoundNo).First().RoundClubs.Add(club);

            _context.SaveChanges();

            game = _context.Games.Where(g => g.GameId == gameId)
                                    .Include(g => g.Rounds)
                                    .ThenInclude(r => r.RoundClubs)
                                    .ThenInclude(r=>r.Club)
                                    .First();

            foreach (var item in game.CurrentRoundClubs)
                newRoundClubs.Add(item.Club.ClubName,item.Club.ClubLogo);

            signal.ChangeRoundClubs(game.P1UserId,(int)game.P2UserId, newRoundClubs, game.CurrentRound.IsP1Turn.Value, game.RoundsWonByFirstPlayer,game.RoundsWonBySecondPlayer);
        }

        public List<RoundClub> SelectClubsForRound()
        {
            List<RoundClub> choosenGameClubs = new List<RoundClub>();
            List<Club> allClubs = _context.Clubs.ToList();
            List<Club> selectedClubs = new List<Club>();
            List<Club> possibleClubsForVerticalAlign = new List<Club>();
            int r;
            for (int i = 0; i <= 3; i++)
            {
                if (i == 0)
                {
                    do
                    {
                        selectedClubs.Clear();
                        choosenGameClubs.Clear();
                        SelectThreeTeams(choosenGameClubs, selectedClubs, allClubs);
                        string club_ids = String.Join(",", choosenGameClubs.Where(p => p.RowNo == i).Select(c => c.ClubId).ToList());
                        possibleClubsForVerticalAlign = _context.Clubs.FromSqlInterpolated($"Generate_Possible_Clubs {club_ids}").ToList();
                    }
                    while (possibleClubsForVerticalAlign.Count() < 3);
                }
                else
                {
                    List<Club> possibleClubThatCanBeSelected = possibleClubsForVerticalAlign.Except(selectedClubs).ToList();
                    r = rnd.Next(possibleClubThatCanBeSelected.Count() - 1);
                    selectedClubs.Add(possibleClubThatCanBeSelected[r]);
                    RoundClub gameClub = new RoundClub();
                    gameClub.ClubId = possibleClubThatCanBeSelected[r].ClubId;
                    gameClub.ColNo = 0;
                    gameClub.RowNo = i;
                    choosenGameClubs.Add(gameClub);
                }
            }
            return choosenGameClubs;
        }

        public void SelectThreeTeams(List<RoundClub> gameC, List<Club> selectedClubs, List<Club> clubs)
        {
            int r;
            for (int j = 1; j <= 3; j++)
            {
                r = rnd.Next(clubs.Count() - selectedClubs.Count() - 1);
                Club club = clubs.Except(selectedClubs).ToList()[r];
                selectedClubs.Add(club);
                RoundClub gameClub = new RoundClub();
                gameClub.ClubId = club.ClubId;
                gameClub.ColNo = j;
                gameClub.RowNo = 0;
                gameC.Add(gameClub);
            }
        }

        [HttpGet]
        public JsonResult MakeMove(int GameId, int? CoordinateX, int? CoordinateY,int? PlayerId,TicTacToeTypes Movetype)
        {
            if(_context.Games.Where(g=>g.GameId==GameId && g.IsBeingPlayed).Count()>0)
            {
                Game thisGame = _context.Games.Where(g => g.GameId == GameId)
                    .Include(p => p.Rounds)
                    .ThenInclude(rc=>rc.RoundClubs)
                    .Include(g=>g.Rounds)
                    .ThenInclude(p=>p.RoundMoves).First();

                bool currenTurnP1 = thisGame.CurrentRound.IsP1Turn.Value;
                thisGame.CurrentRound.IsP1Turn = !thisGame.CurrentRound.IsP1Turn;
                if (CoordinateX==null && CoordinateY == null)
                {
                    _context.SaveChanges();
                    signal.MakeMove(currenTurnP1 ? (int)thisGame.P2UserId : thisGame.P1UserId, null, null, null, false, currenTurnP1, new List<int>());
                    return Json(new { correctMove = false, finishedRound = false, isP1turn = currenTurnP1 });
                }
                if (!thisGame.IsFinished)
                {
                    Round actualRound = thisGame.Rounds.Where(r=>!(bool)r.IsFinished).OrderByDescending(r => r.RoundNo).First();
                    List<RoundClub> gameClubsForTheMove =
                        thisGame.CurrentRoundClubs.Where(gc => gc.ColNo == CoordinateY+1 || gc.RowNo == CoordinateX+1).ToList();

                    bool hasPlayerPlayedForFirstClub = _context.PlayerClubHistories.Where(p => p.PlayerId == PlayerId && p.ClubId == gameClubsForTheMove[0].ClubId).Count() > 0;
                    bool hasPlayerPlayedForSecondClub = _context.PlayerClubHistories.Where(p => p.PlayerId == PlayerId && p.ClubId == gameClubsForTheMove[1].ClubId).Count() > 0;
                    if (hasPlayerPlayedForFirstClub && hasPlayerPlayedForSecondClub)
                        thisGame.CurrentRound.RoundMoves.Add(new RoundMove { ColNo = (int)CoordinateY, RowNo = (int)CoordinateX, CellValue = Movetype.ToString() });
                    else
                    {
                        _context.SaveChanges();
                        signal.MakeMove(currenTurnP1 ? (int)thisGame.P2UserId : thisGame.P1UserId, CoordinateX, CoordinateY, null,false, currenTurnP1, new List<int>());
                        return Json(new { correctMove = false, finishedRound = false, isP1turn = currenTurnP1 });
                    }
                    List<int> isFirstPlayerWinner = functionHelper.CheckIfThereIsAnyWinner(thisGame.CurrentRoundMoves ,TicTacToeTypes.X);
                    List<int> isSecondPlayerWinner = functionHelper.CheckIfThereIsAnyWinner(thisGame.CurrentRoundMoves ,TicTacToeTypes.O);

                    if (isFirstPlayerWinner.Count == 3 || isSecondPlayerWinner.Count == 3)
                        actualRound.IsFinished = true;  
                    if (isFirstPlayerWinner.Count == 3)
                        actualRound.IsP1Win = true;
                    if (isSecondPlayerWinner.Count == 3)
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
                        thisGame.Rounds.Add(new Round { IsFinished = false, IsP1Win = false, RoundNo = roundNo, IsP1Turn = !currenTurnP1 });
                    }

                    _context.SaveChanges();
                    signal.MakeMove(currenTurnP1 ? (int)thisGame.P2UserId : thisGame.P1UserId, CoordinateX, CoordinateY, Movetype, actualRound.IsFinished.Value, currenTurnP1, actualRound.IsP1Win.Value ? isFirstPlayerWinner : isSecondPlayerWinner);
                    return Json(new { correctMove = true, finishedRound = actualRound.IsFinished, isP1turn = currenTurnP1, combination = actualRound.IsP1Win.Value ? isFirstPlayerWinner : isSecondPlayerWinner });
                }
                else
                {
                    _context.SaveChanges();
                    return Json(new { correctMove = false, finishedRound = true, isP1turn = currenTurnP1 });
                }
            }
            else
            {
                return Json(new { correctMove = false, finishedRound = true });
            }
        }

        [HttpGet]
        public List<Player> GetPlayersAutoComplete(string playerName)
        {
            var players = from m in _context.Players
                          where m.PlayerName.Contains(playerName)
                          select m;
            return players.ToList();
        }

        public void SelectPlayer(int userId,string playerName)
        {
            signal.SelectedPlayer(userId, playerName);
        }

        private void SetCookie(string username)
        {
            int playerID = _context.Users.Where(u => u.UserName == username).First().UserId;
            string cookieValue = username + "_" + playerID.ToString();
            string encryptedCookieValue = FunctionHelper.EncryptDecryptValue(true, cookieValue);
            string cookie = Request.Cookies["UC"];
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                Response.Cookies.Delete("UC");
            }
            Response.Cookies.Append("UC", encryptedCookieValue, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddHours(1),
                HttpOnly = true,
                Path = "/"
            });
        }
    }  
}
