
using System;
using System.Collections.Generic;
using Boutique.Entity;

namespace Boutique.Services;

public interface IReviewService
{
    IList<Review> GetReviewsByProductId(Guid productId);
    Review GetReviewByProductIdUserId(Guid productId, Guid userId);
    void InsertReview(Review review);
    void UpdateReview(Review review);
}
