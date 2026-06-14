using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using SuccessArenaBAL.v1;
using SuccessArenaFoundation.Controllers.Base;
using SuccessArenaFoundation.Security;
using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;
using SuccessArenaServiceModels.Foundation.Base.Enums;
using SuccessArenaServiceModels.v1;

namespace SuccessArenaFoundation.Controllers.v1
{

    [ApiController]
    [Route("api/v1/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly QuestionProcess _questionProcess;

        public QuestionController(
            QuestionProcess questionProcess)
        {
            _questionProcess = questionProcess;
        }

        #region Get

        [HttpGet("GetAllQuestions")]
        public async Task<List<QuestionSM>> GetAllQuestions(
            int skip = 0,
            int top = 10)
        {
            return await _questionProcess.GetAllQuestions(
                skip,
                top);
        }

        [HttpGet("GetAllActiveQuestions")]
        public async Task<List<QuestionSM>> GetAllActiveQuestions(
            int skip = 0,
            int top = 10)
        {
            return await _questionProcess.GetAllActiveQuestions(
                skip,
                top);
        }

        [HttpGet("GetQuestionById/{id}")]
        public async Task<QuestionSM> GetQuestionById(
            int id)
        {
            return await _questionProcess.GetQuestionById(id);
        }

        [HttpGet("GetQuestionByIdForUsers/{id}")]
        public async Task<QuestionSM> GetQuestionByIdForUsers(
            int id)
        {
            return await _questionProcess.GetQuestionByIdForUsers(id);
        }

        [HttpGet("GetRandomQuestionsBySubject")]
        public async Task<List<QuestionSM>> GetRandomQuestionsBySubject(
            int subjectId,
            int count)
        {
            return await _questionProcess.GetRandomQuestionsBySubject(
                subjectId,
                count);
        }

        [HttpGet("GetRandomQuestionsByPost")]
        public async Task<List<QuestionSM>> GetRandomQuestionsByPost(
            int postId,
            int count)
        {
            return await _questionProcess.GetRandomQuestionsByPost(
                postId,
                count);
        }

        [HttpGet("GetRandomQuestionsByPostAndSubject")]
        public async Task<List<QuestionSM>> GetRandomQuestionsByPostAndSubject(
            int postId,
            int subjectId,
            int count)
        {
            return await _questionProcess.GetRandomQuestionsByPostAndSubject(
                postId,
                subjectId,
                count);
        }

        [HttpGet("SearchQuestions")]
        public async Task<List<QuestionSM>> SearchQuestions(
            string searchText,
            int skip = 0,
            int top = 10)
        {
            return await _questionProcess.SearchQuestions(
                searchText,
                skip,
                top);
        }

        [HttpGet("GetQuestionsByPost")]
        public async Task<List<QuestionSM>> GetQuestionsByPost(
            int postId,
            int skip = 0,
            int top = 10)
        {
            return await _questionProcess.GetQuestionsByPost(
                postId,
                skip,
                top);
        }

        [HttpGet("GetQuestionsBySubject")]
        public async Task<List<QuestionSM>> GetQuestionsBySubject(
            int subjectId,
            int skip = 0,
            int top = 10)
        {
            return await _questionProcess.GetQuestionsBySubject(
                subjectId,
                skip,
                top);
        }

        [HttpGet("GetQuestionsByPostAndSubject")]
        public async Task<List<QuestionSM>> GetQuestionsByPostAndSubject(
            int postId,
            int subjectId,
            int skip = 0,
            int top = 10)
        {
            return await _questionProcess.GetQuestionsByPostAndSubject(
                postId,
                subjectId,
                skip,
                top);
        }

        #endregion

        #region User Methods

        [HttpGet("GetAvailableQuestionsForUsers")]
        public async Task<List<QuestionSM>> GetAvailableQuestionsForUsers(
            int skip = 0,
            int top = 10)
        {
            return await _questionProcess.GetAvailableQuestionsForUsers(
                skip,
                top);
        }

        [HttpGet("GetQuestionsByPostForUsers")]
        public async Task<List<QuestionSM>> GetQuestionsByPostForUsers(
            int postId,
            int skip = 0,
            int top = 10)
        {
            return await _questionProcess.GetQuestionsByPostForUsers(
                postId,
                skip,
                top);
        }

        [HttpGet("GetQuestionsBySubjectForUsers")]
        public async Task<List<QuestionSM>> GetQuestionsBySubjectForUsers(
            int subjectId,
            int skip = 0,
            int top = 10)
        {
            return await _questionProcess.GetQuestionsBySubjectForUsers(
                subjectId,
                skip,
                top);
        }

        [HttpGet("GetQuestionsByPostAndSubjectForUsers")]
        public async Task<List<QuestionSM>> GetQuestionsByPostAndSubjectForUsers(
            int postId,
            int subjectId,
            int skip = 0,
            int top = 10)
        {
            return await _questionProcess.GetQuestionsByPostAndSubjectForUsers(
                postId,
                subjectId,
                skip,
                top);
        }

        #endregion

        #region Count

        [HttpGet("GetAllQuestionsCount")]
        public async Task<IntResponseRoot> GetAllQuestionsCount()
        {
            return await _questionProcess.GetAllQuestionsCount();
        }

        [HttpGet("GetAllActiveQuestionsCount")]
        public async Task<IntResponseRoot> GetAllActiveQuestionsCount()
        {
            return await _questionProcess.GetAllActiveQuestionsCount();
        }

        [HttpGet("SearchQuestionsCount")]
        public async Task<IntResponseRoot> SearchQuestionsCount(
            string searchText)
        {
            return await _questionProcess.SearchQuestionsCount(searchText);
        }

        [HttpGet("GetQuestionCountByPost")]
        public async Task<IntResponseRoot> GetQuestionCountByPost(
            int postId)
        {
            return await _questionProcess.GetQuestionCountByPost(postId);
        }

        [HttpGet("GetQuestionCountBySubject")]
        public async Task<IntResponseRoot> GetQuestionCountBySubject(
            int subjectId)
        {
            return await _questionProcess.GetQuestionCountBySubject(subjectId);
        }

        [HttpGet("GetQuestionCountByPostAndSubject")]
        public async Task<IntResponseRoot> GetQuestionCountByPostAndSubject(
            int postId,
            int subjectId)
        {
            return await _questionProcess.GetQuestionCountByPostAndSubject(
                postId,
                subjectId);
        }

        [HttpGet("GetActiveQuestionCountByPost")]
        public async Task<IntResponseRoot> GetActiveQuestionCountByPost(
            int postId)
        {
            return await _questionProcess.GetActiveQuestionCountByPost(postId);
        }

        [HttpGet("GetActiveQuestionCountBySubject")]
        public async Task<IntResponseRoot> GetActiveQuestionCountBySubject(
            int subjectId)
        {
            return await _questionProcess.GetActiveQuestionCountBySubject(subjectId);
        }

        [HttpGet("GetActiveQuestionCountByPostAndSubject")]
        public async Task<IntResponseRoot> GetActiveQuestionCountByPostAndSubject(
            int postId,
            int subjectId)
        {
            return await _questionProcess.GetActiveQuestionCountByPostAndSubject(
                postId,
                subjectId);
        }

        [HttpGet("GetAvailableQuestionsForUsersCount")]
        public async Task<IntResponseRoot> GetAvailableQuestionsForUsersCount()
        {
            return await _questionProcess.GetAvailableQuestionsForUsersCount();
        }

        [HttpGet("GetAvailableQuestionCountByPost")]
        public async Task<IntResponseRoot> GetAvailableQuestionCountByPost(
            int postId)
        {
            return await _questionProcess.GetAvailableQuestionCountByPost(postId);
        }

        [HttpGet("GetAvailableQuestionCountBySubject")]
        public async Task<IntResponseRoot> GetAvailableQuestionCountBySubject(
            int subjectId)
        {
            return await _questionProcess.GetAvailableQuestionCountBySubject(subjectId);
        }

        [HttpGet("GetAvailableQuestionCountByPostAndSubject")]
        public async Task<IntResponseRoot> GetAvailableQuestionCountByPostAndSubject(
            int postId,
            int subjectId)
        {
            return await _questionProcess.GetAvailableQuestionCountByPostAndSubject(
                postId,
                subjectId);
        }

        [HttpGet("GetRandomQuestionPoolCount")]
        public async Task<IntResponseRoot> GetRandomQuestionPoolCount(
            int postId,
            int subjectId)
        {
            return await _questionProcess.GetRandomQuestionPoolCount(
                postId,
                subjectId);
        }

        #endregion

        #region Exists

        [HttpGet("QuestionExists/{id}")]
        public async Task<bool> QuestionExists(
            int id)
        {
            return await _questionProcess.QuestionExists(id);
        }

        #endregion

        #region Add

        [HttpPost("AddQuestion")]
        public async Task<QuestionSM> AddQuestion(
            [FromBody] QuestionSM sm)
        {
            return await _questionProcess.AddQuestion(sm);
        }

        #endregion

        #region Update

        [HttpPut("UpdateQuestion/{id}")]
        public async Task<QuestionSM> UpdateQuestion(
            int id,
            [FromBody] QuestionSM sm)
        {
            return await _questionProcess.UpdateQuestion(
                id,
                sm);
        }

        [HttpPut("UpdateStatusOfQuestion")]
        public async Task<BoolResponseRoot> UpdateStatusOfQuestion(
            int id,
            bool status)
        {
            return await _questionProcess.UpdateStatusOfQuestion(
                id,
                status);
        }

        #endregion

        #region Delete

        [HttpDelete("DeleteQuestion/{id}")]
        public async Task<DeleteResponseRoot> DeleteQuestion(
            int id)
        {
            return await _questionProcess.DeleteQuestion(id);
        }

        #endregion
    }
}