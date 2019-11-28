﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.CognitoIdentityProvider.Model;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAdvert.Web.Controllers
{
    public class Accounts : Controller
    {

        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public Accounts(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }

        [HttpGet]
        public async Task<IActionResult> Signup()
        {
            var model = new SignupModel();
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);
                if (user.Status != null)
                {
                    // if user exist
                    ModelState.AddModelError("UserExists", "User with this email already exists");
                    return View(model);
                }

                user.Attributes.Add(CognitoAttribute.Email.AttributeName, model.Email);

                var createdUser = await _userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                if (createdUser.Succeeded)
                {
                    return RedirectToAction("Confirm");
                }
                else
                {
                    foreach (var item in createdUser.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }
                    return View(model);
                }

            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            return View(model);
        }


        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> ConfirmPost(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // user not found

                    ModelState.AddModelError("NotFound", "A user with the given email address was not found");
                    return View(model);
                }


                var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true).ConfigureAwait(false);
                //var result = await _userManager.ConfirmEmailAsync (user, model.Code);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }

                    return View(model);
                }
            }


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Login(LoginModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPost(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("LoginError", "Email and password do not match");
                }
            }

            return View("Login", model);
        }

        [HttpGet]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("ForgetPassword")]
        public async Task<IActionResult> ForgetPasswordPost(ForgetPasswordModel model)
        {
            if (ModelState.IsValid)
            {

                var cognitoUserManager = _userManager as CognitoUserManager<CognitoUser>;

                var cognitoSignInManager = _signInManager as CognitoSignInManager<CognitoUser>;

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)

                {

                    var result = await cognitoUserManager.ResetPasswordAsync(user);

                    if (result.Succeeded)

                        return RedirectToAction("ResetPassword", new { model.Email });

                    else

                        result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));

                }

            }

            return View(model);
        }

        public async Task<IActionResult> ResetPassword(string email)

        {

            var model = new ResetPasswordModel() { Email = email };

            return View(model);

        }



        [HttpPost]

        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)

        {

            if (ModelState.IsValid)

            {

                var user = await _userManager.FindByEmailAsync(model.Email);



                if (user != null)

                {

                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

                    if (result.Succeeded)

                        return RedirectToAction("Login");

                    else

                        result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));

                }

                else

                    ModelState.AddModelError("NotFound", "User not found");

            }



            return View(model);

        }


    }
}
