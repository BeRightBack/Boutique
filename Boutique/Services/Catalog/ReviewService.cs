
using System;
using System.Collections.Generic;
using Boutique.Data;
using Boutique.EFRepository;
using Boutique.Entity;

namespace Boutique.Services;
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member.
public class ReviewService : IReviewService
{
    private readonly IRepository<Review> reviewRepository;

    public ReviewService(        
        IRepository<Review> reviewRepository)
    {        
        this.reviewRepository = reviewRepository;
    }

    public IList<Review> GetReviewsByProductId(Guid productId)
    {
        if (productId == Guid.Empty)
            return null;

        return reviewRepository.FindManyByExpression(x => x.ProductId == productId).ToList();
    }

    public Review GetReviewByProductIdUserId(Guid productId, Guid userId)
    {
        if (productId == Guid.Empty || userId == Guid.Empty)
            return null;

        return reviewRepository.FindByExpression(x => x.ProductId == productId && x.UserId == userId);
    }

    public void InsertReview(Review review)
    {
        if (review == null)
            throw new ArgumentNullException(nameof(review));

        reviewRepository.Insert(review);
        reviewRepository.SaveChanges();
    }

    public void UpdateReview(Review review)
    {
        if (review == null)
            throw new ArgumentNullException(nameof(review));

        reviewRepository.Update(review);
        reviewRepository.SaveChanges();
    }
}
