using Eduprompt.Domain.DTOs.Post;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/posts")]
[ApiExplorerSettings(GroupName = "07. Post")]
[Produces("application/json")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// Get all posts
    /// </summary>
    /// <returns>List of all posts</returns>
    /// <response code="200">Posts retrieved successfully</response>
    /// <response code="400">Error retrieving posts</response>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var posts = await _postService.GetAllAsync();
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get post by ID
    /// </summary>
    /// <param name="PostId">Post ID</param>
    /// <returns>Post details</returns>
    /// <response code="200">Post found</response>
    /// <response code="400">Error retrieving post</response>
    /// <response code="404">Post not found</response>
    [HttpGet("{PostId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int PostId)
    {
        try
        {
            var post = await _postService.GetByIdAsync(PostId);
            if (post == null)
                return NotFound(new { message = "Post not found" });

            // Increment view count
            await _postService.IncrementViewCountAsync(PostId);

            return Ok(post);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy bài đăng theo User ID
    /// </summary>
    [HttpGet("user/{UserId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByUserId(int UserId)
    {
        try
        {
            var posts = await _postService.GetByUserIdAsync(UserId);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy bài đăng đã xuất bản
    /// </summary>
    [HttpGet("published")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublishedPosts()
    {
        try
        {
            var posts = await _postService.GetPublishedPostsAsync();
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy bài đăng theo loại
    /// </summary>
    [HttpGet("type/{postType}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByPostType(string postType)
    {
        try
        {
            var posts = await _postService.GetByPostTypeAsync(postType);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tìm kiếm bài đăng
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] string searchTerm)
    {
        try
        {
            if (string.IsNullOrEmpty(searchTerm))
                return BadRequest(new { message = "Search term is required" });

            var posts = await _postService.SearchAsync(searchTerm);
            return Ok(posts);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Tạo bài đăng mới
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreatePostDto createPostDto)
    {
        try
        {
            // Verify userId from token matches request
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Missing or invalid user claim" });
            }

            // Override UserId from token to prevent user creating post for others
            createPostDto.UserId = userId;

            var post = await _postService.CreateAsync(createPostDto);
            return CreatedAtAction(nameof(GetById), new { PostId = post.PostId }, post);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật bài đăng (only owner or admin)
    /// </summary>
    [HttpPut("{PostId}")]
    [Authorize]
    public async Task<IActionResult> Update(int PostId, [FromBody] CreatePostDto updatePostDto)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Missing or invalid user claim" });
            }

            // Verify ownership
            var existingPost = await _postService.GetByIdAsync(PostId);
            if (existingPost == null)
                return NotFound(new { message = "Post not found" });

            if (existingPost.UserId != userId && !User.IsInRole("Admin"))
                return StatusCode(403, new { message = "You can only update your own posts" });

            // Override UserId from token to prevent user updating post for others
            updatePostDto.UserId = existingPost.UserId;

            var post = await _postService.UpdateAsync(PostId, updatePostDto);
            return Ok(post);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Xóa bài đăng (only owner or admin)
    /// </summary>
    [HttpDelete("{PostId}")]
    [Authorize]
    public async Task<IActionResult> Delete(int PostId)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Missing or invalid user claim" });
            }

            // Verify ownership
            var existingPost = await _postService.GetByIdAsync(PostId);
            if (existingPost == null)
                return NotFound(new { message = "Post not found" });

            if (existingPost.UserId != userId && !User.IsInRole("Admin"))
                return StatusCode(403, new { message = "You can only delete your own posts" });

            var result = await _postService.DeleteAsync(PostId);
            if (!result)
                return NotFound(new { message = "Post not found" });

            return Ok(new { message = "Post deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Like bài đăng
    /// </summary>
    [HttpPost("{PostId}/like")]
    [Authorize]
    public async Task<IActionResult> LikePost(int PostId)
    {
        try
        {
            var result = await _postService.IncrementLikeCountAsync(PostId);
            if (!result)
                return NotFound(new { message = "Post not found" });

            return Ok(new { message = "Post liked successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy đánh giá trung bình của bài đăng
    /// </summary>
    [HttpGet("{PostId}/rating")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAverageRating(int PostId)
    {
        try
        {
            var rating = await _postService.GetAverageRatingAsync(PostId);
            return Ok(new { averageRating = rating });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Purchase template from post
    /// </summary>
    /// <param name="PostId">Post ID</param>
    /// <returns>Purchase result with StorageId and PromptInstanceId</returns>
    /// <response code="200">Purchase completed successfully</response>
    /// <response code="400">Invalid request or purchase failed</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Post not found</response>
    [HttpPost("{PostId}/purchase")]
    [Authorize]
    public async Task<IActionResult> PurchasePost(int PostId)
    {
        try
        {
            var buyerUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var result = await _postService.PurchasePostAsync(PostId, buyerUserId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
