﻿using System.Diagnostics;
using System.Drawing;
using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Web.HttpUtility;

namespace Chirp.Web.Pages;

public class PublicModel : BaseModel
{


    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IFollowRepository followRepository,
IReactionRepository reactionRepository) : base(cheepRepository, authorRepository, followRepository, reactionRepository)
    {

    }

    public IEnumerable<CheepDTO> Cheeps { get; set; }
    public IEnumerable<CheepInfoDTO> CheepInfos { get; set; }
    public int pageNr { get; set; }
    public int pages { get; set; }
    public string TotalReactions { get; set; }


    public async Task<ActionResult> OnGetAsync()
    {
        List<CheepInfoDTO> CheepInfoList = new List<CheepInfoDTO>();

        // get user

        var Claims = User.Claims;

        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        var username = Claims.FirstOrDefault(c => c.Type == "name")?.Value;

        if (User.Identity?.IsAuthenticated == true && (currentlyLoggedInUser == null || currentlyLoggedInUser.Name == null))
        {
            try
            {
                if (email != null && await _authorRepository.GetAuthorByEmailAsync(email) == null)
                {
                    await _authorRepository.InsertAuthorAsync(username, email, "ONLINE");
                    await _authorRepository.SaveAsync();
                    currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

                }
                else
                {
                    currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("author insert failed");
            }
        }
        else if (User.Identity?.IsAuthenticated == true)
        {
            await _authorRepository.UpdateAuthorStatusAsync(email);
        }




        // pages = _service.getPagesHome(false, null);
        pages = _cheepRepository.getPages();
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));
        Cheeps = _cheepRepository.GetCheeps(pageNr);



        if (currentlyLoggedInUser != null)
        {
            currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

            //To get the CheepInfos we need to do some work...
            List<string> followingIDs = await _followRepository.GetFollowingIDsByAuthorIDAsync(currentlyLoggedInUser.AuthorId);
            List<string> reactionCheepIds = await _reactionRepository.GetCheepIdsByAuthorId(currentlyLoggedInUser.AuthorId);


            foreach (CheepDTO cheep in Cheeps)
            {
                CheepInfoDTO cheepInfoDTO = new CheepInfoDTO
                {
                    Cheep = cheep,
                    UserIsFollowingAuthor = IsUserFollowingAuthor(cheep.AuthorId, followingIDs),
                    UserReactToCheep = IsUserReactionCheep(cheep.Id, reactionCheepIds),
                    TotalReactions = await getTotalReactions(cheep.Id),

                };
                CheepInfoList.Add(cheepInfoDTO);
            }

        }



        var viewModel = new ViewModel
        {
            pageNr = pageNr,
            pages = pages,
            CheepInfos = CheepInfoList,
            Cheeps = Cheeps,
            User = currentlyLoggedInUser,
        };

        ViewData["ViewModel"] = viewModel;


        return Page();
    }

    public async Task<string> getTotalReactions(string cheepId)
    {
        var total = _reactionRepository.GetReactionByCheepId(cheepId);
        var totalLikes = total.Result.Count().ToString();
        if (totalLikes == "0")
        {
            return "0 Likes";
        }
        else if (totalLikes == "1")
        {
            return "1 Like";
        }
        else
        {
            return totalLikes + " Likes";
        }
    }

}

