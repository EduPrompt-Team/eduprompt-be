using Eduprompt.Domain.DTOs.Post;
using Eduprompt.Domain.Interface.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eduprompt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    /// <param name="postId">Post ID</param>
    /// <returns>Post details</returns>
    /// <response code="200">Post found</response>
    /// <response code="400">Error retrieving post</response>
    /// <response code="404">Post not found</response>
    [HttpGet("{postId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int postId)
    {
        try
        {
            var post = await _postService.GetByIdAsync(postId);
            if (post == null)
                return NotFound(new { message = "Post not found" });

            // Increment view count
            await _postService.IncrementViewCountAsync(postId);

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
    [HttpGet("user/{userId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        try
        {
            var posts = await _postService.GetByUserIdAsync(userId);
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
            var post = await _postService.CreateAsync(createPostDto);
            return CreatedAtAction(nameof(GetById), new { postId = post.PostID }, post);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cập nhật bài đăng
    /// </summary>
    [HttpPut("{postId}")]
    [Authorize]
    public async Task<IActionResult> Update(int postId, [FromBody] CreatePostDto updatePostDto)
    {
        try
        {
            var post = await _postService.UpdateAsync(postId, updatePostDto);
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
    /// Xóa bài đăng
    /// </summary>
    [HttpDelete("{postId}")]
    [Authorize]
    public async Task<IActionResult> Delete(int postId)
    {
        try
        {
            var result = await _postService.DeleteAsync(postId);
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
    [HttpPost("{postId}/like")]
    [Authorize]
    public async Task<IActionResult> LikePost(int postId)
    {
        try
        {
            var result = await _postService.IncrementLikeCountAsync(postId);
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
    [HttpGet("{postId}/rating")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAverageRating(int postId)
    {
        try
        {
            var rating = await _postService.GetAverageRatingAsync(postId);
            return Ok(new { averageRating = rating });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
