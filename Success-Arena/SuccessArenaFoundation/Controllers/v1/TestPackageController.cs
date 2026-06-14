using Microsoft.AspNetCore.Mvc;
using SuccessArenaBAL.v1;
using SuccessArenaServiceModels.Foundation.Base.CommonResponseRoot;
using SuccessArenaServiceModels.v1;

namespace SuccessArenaFoundation.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestPackageController : ControllerBase
    {
        private readonly TestPackageProcess _testPackageProcess;

        public TestPackageController(
            TestPackageProcess testPackageProcess)
        {
            _testPackageProcess = testPackageProcess;
        }

        #region Get

        [HttpGet("GetAllTestPackages")]
        public async Task<List<TestPackageSM>> GetAllTestPackages(
            int skip = 0,
            int top = 10)
        {
            return await _testPackageProcess.GetAllTestPackages(
                skip,
                top);
        }

        [HttpGet("GetActiveTestPackages")]
        public async Task<List<TestPackageSM>> GetActiveTestPackages(
            int skip = 0,
            int top = 10)
        {
            return await _testPackageProcess.GetActiveTestPackages(
                skip,
                top);
        }

        [HttpGet("GetTestPackagesByPost")]
        public async Task<List<TestPackageSM>> GetTestPackagesByPost(
            int postId,
            int skip = 0,
            int top = 10)
        {
            return await _testPackageProcess.GetTestPackagesByPost(
                postId,
                skip,
                top);
        }

        [HttpGet("GetTestPackagesByPostAndSubject")]
        public async Task<List<TestPackageSM>> GetTestPackagesByPostAndSubject(
            int postId,
            int subjectId,
            int skip = 0,
            int top = 10)
        {
            return await _testPackageProcess.GetTestPackagesByPostAndSubject(
                postId,
                subjectId,
                skip,
                top);
        }

        [HttpGet("GetAvailableTestPackagesForUsers")]
        public async Task<List<TestPackageSM>> GetAvailableTestPackagesForUsers(
            int skip = 0,
            int top = 10)
        {
            return await _testPackageProcess.GetAvailableTestPackagesForUsers(
                skip,
                top);
        }

        [HttpGet("GetTestPackagesByPostForUsers")]
        public async Task<List<TestPackageSM>> GetTestPackagesByPostForUsers(
            int postId,
            int skip = 0,
            int top = 10)
        {
            return await _testPackageProcess.GetTestPackagesByPostForUsers(
                postId,
                skip,
                top);
        }

        [HttpGet("GetTestPackagesByPostAndSubjectForUsers")]
        public async Task<List<TestPackageSM>> GetTestPackagesByPostAndSubjectForUsers(
            int postId,
            int subjectId,
            int skip = 0,
            int top = 10)
        {
            return await _testPackageProcess.GetTestPackagesByPostAndSubjectForUsers(
                postId,
                subjectId,
                skip,
                top);
        }

        [HttpGet("GetFreeTestPackagesForUsers")]
        public async Task<List<TestPackageSM>> GetFreeTestPackagesForUsers(
            int skip = 0,
            int top = 10)
        {
            return await _testPackageProcess.GetFreeTestPackagesForUsers(
                skip,
                top);
        }

        [HttpGet("GetTestPackageById/{id}")]
        public async Task<TestPackageSM> GetTestPackageById(
            int id)
        {
            return await _testPackageProcess.GetTestPackageById(id);
        }

        [HttpGet("GetTestPackageByIdForUsers/{id}")]
        public async Task<TestPackageSM> GetTestPackageByIdForUsers(
            int id)
        {
            return await _testPackageProcess.GetTestPackageByIdForUsers(id);
        }

        #endregion

        #region Count

        [HttpGet("GetAllTestPackagesCount")]
        public async Task<IntResponseRoot> GetAllTestPackagesCount()
        {
            return await _testPackageProcess.GetAllTestPackagesCount();
        }

        [HttpGet("GetActiveTestPackagesCount")]
        public async Task<IntResponseRoot> GetActiveTestPackagesCount()
        {
            return await _testPackageProcess.GetActiveTestPackagesCount();
        }

        [HttpGet("GetTestPackageCountByPost")]
        public async Task<IntResponseRoot> GetTestPackageCountByPost(
            int postId)
        {
            return await _testPackageProcess.GetTestPackageCountByPost(
                postId);
        }

        [HttpGet("GetTestPackageCountByPostAndSubject")]
        public async Task<IntResponseRoot> GetTestPackageCountByPostAndSubject(
            int postId,
            int subjectId)
        {
            return await _testPackageProcess.GetTestPackageCountByPostAndSubject(
                postId,
                subjectId);
        }

        [HttpGet("GetAvailableTestPackageCountByPost")]
        public async Task<IntResponseRoot> GetAvailableTestPackageCountByPost(
            int postId)
        {
            return await _testPackageProcess.GetAvailableTestPackageCountByPost(
                postId);
        }

        [HttpGet("GetAvailableTestPackageCountByPostAndSubject")]
        public async Task<IntResponseRoot> GetAvailableTestPackageCountByPostAndSubject(
            int postId,
            int subjectId)
        {
            return await _testPackageProcess.GetAvailableTestPackageCountByPostAndSubject(
                postId,
                subjectId);
        }

        #endregion

        #region Add

        [HttpPost("AddTestPackage")]
        public async Task<TestPackageSM> AddTestPackage(
            [FromBody] TestPackageSM sm)
        {
            return await _testPackageProcess.AddTestPackage(sm);
        }

        #endregion

        #region Update

        [HttpPut("UpdateTestPackage/{id}")]
        public async Task<TestPackageSM> UpdateTestPackage(
            int id,
            [FromBody] TestPackageSM sm)
        {
            return await _testPackageProcess.UpdateTestPackage(
                id,
                sm);
        }

        [HttpPut("UpdateStatusOfTestPackage")]
        public async Task<BoolResponseRoot> UpdateStatusOfTestPackage(
            int id,
            bool status)
        {
            return await _testPackageProcess.UpdateStatusOfTestPackage(
                id,
                status);
        }

        #endregion

        #region Delete

        [HttpDelete("DeleteTestPackage/{id}")]
        public async Task<DeleteResponseRoot> DeleteTestPackage(
            int id)
        {
            return await _testPackageProcess.DeleteTestPackage(id);
        }

        #endregion
    }
}