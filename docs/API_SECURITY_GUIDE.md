# API Security Implementation Guide

**Issue**: [#19 - API Security: Implement JWT Authentication and Authorization](https://github.com/dkennyq/bla-task-management-system/issues/19)  
**Priority**: HIGH (blocks frontend development)  
**Estimated Effort**: 6-9 hours  
**Created**: 2026-06-10

---

## 🎯 Quick Summary

Implement JWT-based authentication to secure communication between the Vue.js frontend and .NET backend APIs before starting frontend development.

**What's Being Protected:**
- ✅ Tasks API endpoints (all CRUD operations)
- ✅ Users API endpoints (profile, list users)

**Current State**: APIs are unprotected - anyone can access any data  
**Target State**: JWT token required for all protected endpoints

---

## 📋 Implementation Phases

### Phase 1: Backend - Users API (2-3 hours)
**Goal**: Generate JWT tokens on login

**Tasks:**
1. Add `Microsoft.AspNetCore.Authentication.JwtBearer` NuGet package
2. Create `JwtTokenService.cs` in Application layer
3. Update login endpoint to return token
4. Configure JWT settings in `appsettings.json`
5. Test with Postman/curl

**Key Files:**
```
apps/users-api/src/UsersApi.Application/Services/JwtTokenService.cs
apps/users-api/src/UsersApi.WebApi/Program.cs
apps/users-api/src/UsersApi.WebApi/appsettings.json
```

**Test Command:**
```bash
curl -X POST http://localhost:5078/api/users/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@taskmanagement.com","password":"Password123!"}'

# Should return: { "token": "eyJhbG...", "expiresAt": "..." }
```

---

### Phase 2: Backend - Tasks API (1-2 hours)
**Goal**: Protect task endpoints with JWT validation

**Tasks:**
1. Add JWT authentication middleware to `Program.cs`
2. Add `[Authorize]` attribute to TasksController
3. Extract userId from JWT claims (not query params)
4. Configure same JWT settings as Users API
5. Update Swagger for JWT authorization
6. Test with token from Phase 1

**Key Changes:**
```csharp
// BEFORE (insecure)
[HttpGet]
public async Task<IActionResult> GetAll([FromQuery] Guid userId)

// AFTER (secure)
[Authorize]
[HttpGet]
public async Task<IActionResult> GetAll()
{
    var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    // ...
}
```

**Test Command:**
```bash
# Without token (should fail)
curl http://localhost:5077/api/tasks
# Expected: 401 Unauthorized

# With token (should succeed)
curl http://localhost:5077/api/tasks \
  -H "Authorization: Bearer eyJhbG..."
# Expected: 200 OK
```

---

### Phase 3: Frontend (2-3 hours)
**Goal**: Handle authentication in Vue.js application

**Tasks:**
1. Update auth store to manage token
2. Create axios interceptor for Authorization header
3. Implement router guards for protected routes
4. Handle 401 responses (logout + redirect)
5. Update login page to store token
6. Test end-to-end flow

**Key Files:**
```
apps/web/src/stores/auth.js
apps/web/src/services/api.js
apps/web/src/router/index.js
```

**Flow:**
```
Login → Get token → Store in localStorage → 
Add to headers → Access protected routes → 
401? → Logout → Redirect to login
```

---

### Phase 4: Documentation (30 min)
**Tasks:**
1. Update `docs/TESTING_APIS.md` with JWT examples
2. Update Postman collection with token variable
3. Add authentication section to `docs/USER_STORIES.md`
4. Update README if needed

---

## 🔐 JWT Token Structure

**Claims:**
```json
{
  "sub": "user-id-here",
  "email": "user@example.com",
  "name": "John Doe",
  "iat": 1234567890,
  "exp": 1234654290,
  "iss": "TaskManagementAPI",
  "aud": "TaskManagementWeb"
}
```

**Expiration**: 24 hours (configurable)

---

## ✅ Verification Checklist

### Backend
- [ ] Login returns valid JWT token
- [ ] Token includes userId in `sub` claim
- [ ] Tasks API rejects requests without token (401)
- [ ] Tasks API accepts requests with valid token (200)
- [ ] Token expiration works correctly
- [ ] Swagger shows "Authorize" button
- [ ] All tests pass with authentication

### Frontend
- [ ] Login stores token in localStorage
- [ ] API requests include Authorization header
- [ ] 401 responses trigger logout
- [ ] Protected routes redirect to login when not authenticated
- [ ] Token persists across page refreshes
- [ ] Logout clears token and redirects

### Integration
- [ ] End-to-end flow works: Login → Access task → Logout
- [ ] Token expiration handled gracefully
- [ ] Multiple API calls reuse same token
- [ ] Postman collection works with new auth

---

## 🚨 Security Best Practices

### DO ✅
- Store JWT secret in environment variables
- Use HTTPS in production
- Set reasonable token expiration (24h)
- Hash passwords with bcrypt (already implemented)
- Validate token signature and expiration
- Use CORS properly

### DON'T ❌
- Store secrets in source code
- Store tokens in cookies without httpOnly flag
- Trust client-provided userId
- Use weak JWT secrets
- Skip token expiration checks
- Expose sensitive data in token claims

---

## 🔗 Related Resources

**Documentation:**
- [JWT.io - Token Debugger](https://jwt.io/)
- [Microsoft JWT Bearer Docs](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
- [Vue.js Security Best Practices](https://vuejs.org/guide/best-practices/security.html)

**Related Issues:**
- Issue #5: US-07 - User Registration
- Issue #6: US-08 - User Login (needs update)
- Issue #9: US-13 - Login Page (needs update)
- Issues #1-4: Task endpoints (need protection)

**Agent Prompts:**
- `docs/AGENT_PROMPT_BACKEND.md` - Backend implementation guide
- `docs/AGENT_PROMPT_FRONTEND.md` - Frontend implementation guide

---

## 📞 Agent Delegation

### Backend Agent Prompt:
```
Implement Issue #19: API Security - JWT Authentication

Repo: C:\Users\devke\source\bla-task-management-system

Phase 1 & 2 (Backend):
1. Read issue #19 completely
2. Implement JWT token generation in Users API
3. Protect Tasks API endpoints with [Authorize]
4. Extract userId from JWT claims
5. Update tests to include authentication
6. Verify with Postman/curl

Requirements:
- TDD approach
- Clean Architecture
- All acceptance criteria met
- Documentation updated

Start with: gh issue view 19
```

### Frontend Agent Prompt:
```
Implement Issue #19: API Security - Frontend Authentication

Repo: C:\Users\devke\source\bla-task-management-system

Phase 3 (Frontend):
1. Read issue #19 completely
2. Update auth store for token management
3. Add axios interceptors for Authorization header
4. Implement router guards
5. Handle 401 responses gracefully
6. Test end-to-end authentication flow

Requirements:
- Vue.js 3 Composition API
- TDD with Vitest
- All acceptance criteria met

Start with: gh issue view 19
```

---

**Status**: Issue created and added to GitHub Project  
**URL**: https://github.com/dkennyq/bla-task-management-system/issues/19  
**Next Steps**: Implement backend authentication before starting frontend issues #9 and #10
