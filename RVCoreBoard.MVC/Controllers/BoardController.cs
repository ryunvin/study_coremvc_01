﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RVCoreBoard.MVC.DataContext;
using RVCoreBoard.MVC.Models;
using RVCoreBoard.MVC.Services;

namespace RVCoreBoard.MVC.Controllers
{
    public class BoardController : Controller
    {
        private readonly RVCoreBoardDBContext _db;
        private IBoardService _boardService;

        public BoardController(RVCoreBoardDBContext db, IBoardService boardService)
        {
            _db = db;
            _boardService = boardService;
        }

        /// <summary>
        /// 게시판 리스트
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            BoardListInfoModel boardListInfoModel = new BoardListInfoModel(_boardService);
            await boardListInfoModel.GetList();

            return View(boardListInfoModel);
        }

        /// <summary>
        /// 게시물 목록
        /// </summary>
        /// <param name="BNo"></param>
        /// <returns></returns>
        public IActionResult Detail(int BNo)
        {
            Board board = _db.Boards.FirstOrDefault(b => b.BNo.Equals(BNo));

            board.Cnt_Read++;

            _db.Entry(board).State = EntityState.Modified;
            _db.SaveChanges();

            return View(board);
        }
        /// <summary>
        /// 게시물 추가
        /// </summary>
        /// <returns></returns>
        public IActionResult Add()
        {
            if (HttpContext.Session.GetInt32("USER_LOGIN_KEY") == null)
            {
                //로그인이 안된 상태
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Add(Board model)
        {
            if (HttpContext.Session.GetInt32("USER_LOGIN_KEY") == null)
            {
                //로그인이 안된 상태
                return RedirectToAction("Login", "Account");
            }

            model.UNo = int.Parse(HttpContext.Session.GetInt32("USER_LOGIN_KEY").ToString());
            model.Reg_Date = DateTime.Now;
            model.Cnt_Read = 0;

            if (ModelState.IsValid)
            {
                _db.Boards.Add(model);
                if (_db.SaveChanges() > 0)
                {
                    return Redirect("Index");
                }

                ModelState.AddModelError(string.Empty, "게시물을 등록할 수 없습니다.");
            }
            return View(model);
        }

        /// <summary>
        /// 게시물 수정
        /// </summary>
        /// <returns></returns>
        public IActionResult Edit(int BNo)
        {
            if (HttpContext.Session.GetInt32("USER_LOGIN_KEY") == null)
            {
                //로그인이 안된 상태
                return RedirectToAction("Login", "Account");
            }
            var Board = _db.Boards.FirstOrDefault(b => b.BNo.Equals(BNo));

            return View(Board);
        }

        [HttpPost]
        public IActionResult Edit(Board model)
        {
            if (HttpContext.Session.GetInt32("USER_LOGIN_KEY") == null)
            {
                //로그인이 안된 상태
                return RedirectToAction("Login", "Account");
            }

            model.UNo = int.Parse(HttpContext.Session.GetInt32("USER_LOGIN_KEY").ToString());

            if (ModelState.IsValid)
            {
                _db.Entry(model).State = EntityState.Modified;
                if (_db.SaveChanges() > 0)
                {
                    return Redirect($"Detail?BNo={model.BNo}");
                }
                ModelState.AddModelError(string.Empty, "게시물을 수정할 수 없습니다.");
            }
            return View(model);
        }

        /// <summary>
        /// 게시물 삭제 
        /// </summary>
        /// <returns></returns>
        public IActionResult Delete(int BNo)
        {
            var Board = _db.Boards.FirstOrDefault(b => b.BNo.Equals(BNo));

            _db.Boards.Remove(Board);
            if (_db.SaveChanges() > 0)
            {
                return Redirect("Index");
            }
            return Redirect($"Detail?BNo={BNo}");
        }
    }
}
