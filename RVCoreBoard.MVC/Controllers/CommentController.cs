﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RVCoreBoard.MVC.Attributes;
using RVCoreBoard.MVC.DataContext;
using RVCoreBoard.MVC.Models;
using static RVCoreBoard.MVC.Models.User;

namespace RVCoreBoard.MVC.Controllers
{
    public class CommentController : Controller
    {
        private readonly RVCoreBoardDBContext _db;

        public CommentController(RVCoreBoardDBContext db)
        {
            _db = db;
        }

        [HttpPost, Route("api/commentAdd")]
        [CustomAuthorize(RoleEnum = UserLevel.Junior | UserLevel.Senior | UserLevel.Senior | UserLevel.Admin)]
        public async Task<IActionResult> CommentAdd(Comment comment)
        {
            comment.Reg_Date = DateTime.Now;

            await _db.Comments.AddAsync(comment);
            if (await _db.SaveChangesAsync() > 0)
            {
                var cment = await _db.Comments.OrderByDescending(c => c.CNo).FirstOrDefaultAsync();

                return Ok(cment);
            }
            return NotFound();
        }

        [HttpPost, Route("api/commentDelete")]
        [CustomAuthorize(RoleEnum = UserLevel.Junior | UserLevel.Senior | UserLevel.Senior | UserLevel.Admin)]
        public async Task<IActionResult> CommentDelete(string CNo)
        {
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.CNo.Equals(int.Parse(CNo)));

            _db.Comments.Remove(comment);
            if (_db.SaveChanges() > 0)
            {
                return Json(new { success = true, responseText = "삭제되었습니다." });
            }
            return Json(new { success = false, responseText = "오류 : 삭제되지 않았습니다." });
        }

        [HttpPost, Route("api/commentModify")]
        [CustomAuthorize(RoleEnum = UserLevel.Junior | UserLevel.Senior | UserLevel.Senior | UserLevel.Admin)]
        public async Task<IActionResult> CommentModify(Comment comment)
        {
            comment.Reg_Date = DateTime.Now;

            _db.Entry(comment).State = EntityState.Modified;
            if (await _db.SaveChangesAsync() > 0)
            {
                var cment = await _db.Comments
                                    .Include("user")
                                    .FirstOrDefaultAsync(c => c.CNo.Equals(comment.CNo));

                return Ok(cment);
            }
            return NotFound();
        }
    }
}