using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Shouldly;
using Xunit;

namespace PossibleBugsV8
{
    public class TweetDerivedCollectionTests
    {
        [Fact()]
        [Trait("Category", "Instant")]
        public void MethodName()
        {
            var sut = new TweetsListViewModel();
            sut.TweetTiles.ChangeTrackingEnabled = true;
            sut.Tweets.Add(new Tweet());

            sut.Tweets.Count.ShouldBe(1);
            sut.TweetTiles.Count.ShouldBe(1);
            sut.VisibleTiles.Count.ShouldBe(1);

            sut.TweetTiles[0].IsHidden = true;
            sut.Tweets.Count.ShouldBe(1);
            sut.TweetTiles.Count.ShouldBe(1);
            sut.VisibleTiles.Count.ShouldBe(0); //This line fails.
        }
    }

    public class Tweet
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class TweetTileViewModel : ReactiveObject
    {
        bool isHidden;
        public bool IsHidden
        {
            get { return isHidden; }
            set { this.RaiseAndSetIfChanged(ref isHidden, value); }
        }

        public Tweet Model { get; set; }
    }

    public class TweetsListViewModel : ReactiveObject
    {
        public ReactiveList<Tweet> Tweets = new ReactiveList<Tweet>();

        public IReactiveDerivedList<TweetTileViewModel> TweetTiles;
        public IReactiveDerivedList<TweetTileViewModel> VisibleTiles;

        public TweetsListViewModel()
        {
            TweetTiles = Tweets.CreateDerivedCollection(
                x => new TweetTileViewModel() { Model = x },
                x => true,
                (x, y) => x.Model.CreatedAt.CompareTo(y.Model.CreatedAt));

            VisibleTiles = TweetTiles.CreateDerivedCollection(
                x => x,
                x => !x.IsHidden);
        }
    }
}
