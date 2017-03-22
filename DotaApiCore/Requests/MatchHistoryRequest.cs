﻿using System.IO;
using System.Net.Http;
using DotaApiCore.MatchHistory.Models;
using Newtonsoft.Json.Linq;

namespace DotaApiCore.Requests
{
    
    public class MatchHistoryRequest : Request
    {
        /* 
         * NOTE: LeagueID to be implemented as part of the "League sprint"
         * Omitting for now
         * Same situation for Tourney games only filter
         */

        private string MatchHistoryBaseUrl { get; set; }

        public long? AccountId { get; set; }

        //TODO: Enumerize
        public int? HeroId { get; set; }

        //TODO: Enumerize
        public int? GameMode { get; set; }

        //TODO: Enumerize
        public int? Skill { get; set; }

        public int? MinimumPlayers { get; set; }

        public long? StartAtMatchId { get; set; }

        public int? MatchesRequested { get; set; }

        public MatchHistoryRequest(string apiKey, long? accountId = null, int? heroId = null, int? gameMode = null, int? skill = null, 
            int? minPlayers = null, long? startingMatchId = null, int? matchesRequested = 100)
        {
            ApiKey = apiKey;
            AccountId = accountId;
            HeroId = heroId;
            GameMode = gameMode;
            Skill = skill;
            MinimumPlayers = minPlayers;
            StartAtMatchId = startingMatchId;
            MatchesRequested = matchesRequested;

            var config = JObject.Parse(File.ReadAllText(@"C:\Projects\DotaApi\DotaApiCore\config.json"));
            var urlConfig = config.ToObject<UrlConfiguration>();
            MatchHistoryBaseUrl = InitializeUrl(urlConfig);
        }

        protected sealed override string InitializeUrl(UrlConfiguration urlConfig)
        {
           return urlConfig.BaseUrl + urlConfig.GetMatchHistoryExtension + string.Format("?key={0}", ApiKey);
        }

        public override HttpResponseMessage SendRequest()
        {
            var requestUrl = BuildUrlParameters(MatchHistoryBaseUrl);

            var client = new HttpClient();
            var result = client.GetAsync(requestUrl).Result;

            return result;
        }

        private string BuildUrlParameters(string requestUrl)
        {
            if (AccountId != null)
            {
                requestUrl = requestUrl + string.Format("&account_Id={0}", AccountId);
            }

            if (HeroId != null)
            {
                requestUrl = requestUrl + string.Format("&hero_id={0}", HeroId);
            }

            if (GameMode != null)
            {
                requestUrl = requestUrl + string.Format("&game_mode={0}", GameMode);
            }

            if (Skill != null)
            {
                requestUrl = requestUrl + string.Format("&skill={0}", Skill);
            }

            if (MinimumPlayers != null)
            {
                requestUrl = requestUrl + string.Format("&min_players={0}", MinimumPlayers);
            }

            if (StartAtMatchId != null)
            {
                requestUrl = requestUrl + string.Format("&start_at_match_id={0}", StartAtMatchId);
            }

            if (MatchesRequested != null)
            {
                requestUrl = requestUrl + string.Format("&matches_requested={0}", MatchesRequested);
            }

            return requestUrl;
        }
    }
}