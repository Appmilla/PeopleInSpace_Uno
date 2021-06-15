using PeopleInSpace_Uno.SharedFeatures.Models;
using PeopleInSpace_Uno.SharedFeatures.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeopleInSpace_Uno.SharedFeatures.Queries
{
    public interface IPeopleInSpaceQuery
    {
        bool IsBusy { get; set; }

        IObservable<ICollection<CrewModel>> GetCrew(bool forceRefresh = false);
    }

    public class PeopleInSpaceQuery : ReactiveObject, IPeopleInSpaceQuery
    {
        ISchedulerProvider _schedulerProvider;

        [Reactive]
        public bool IsBusy { get; set; }

        List<CrewModel> _crew = new List<CrewModel>();

        public PeopleInSpaceQuery(ISchedulerProvider schedulerProvider)
        {
            _schedulerProvider = schedulerProvider;

            _crew.Add(new CrewModel
            {
                Name = "Robert Behnken",
                Agency = Agency.Nasa,
                Image = new Uri("https://imgur.com/0smMgMH.png"),
                Wikipedia = new Uri("https://en.wikipedia.org/wiki/Robert_L._Behnken"),
                Launches = new Launch[] { Launch.The5Eb87D46Ffd86E000604B388 },
                Status = Status.Active,
                Id = "5ebf1a6e23a9a60006e03a7a"
            });
            _crew.Add(new CrewModel
            {
                Name = "Douglas Hurley",
                Agency = Agency.Nasa,
                Image = new Uri("https://i.imgur.com/ooaayWf.png"),
                Wikipedia = new Uri("https://en.wikipedia.org/wiki/Douglas_G._Hurley"),
                Launches = new Launch[] { Launch.The5Eb87D46Ffd86E000604B388 },
                Status = Status.Active,
                Id = "5ebf1a6e23a9a60006e03a7a"
            });
        }

        public IObservable<ICollection<CrewModel>> GetCrew(bool forceRefresh = false)
        {
            return Observable.Create<ICollection<CrewModel>>(async observer =>
            {
                var results = await GetCrewAsync(forceRefresh).ConfigureAwait(false);

                observer.OnNext(results);

            }).SubscribeOn(_schedulerProvider.MainThread);
        }

        async Task<ICollection<CrewModel>> GetCrewAsync(bool forceRefresh = false)
        {
            IsBusy = true;

            await Task.Delay(500).ConfigureAwait(false);
            
            IsBusy = false;

            return _crew;
        }
    }
}
