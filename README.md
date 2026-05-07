# Lottery Ticket Recording System

## Production Deployment

### Architecture
- Frontend: Angular 18 deployed on Vercel
- Backend: ASP.NET Core 8 API deployed on Render (Docker)
- Database: Supabase PostgreSQL

### Files Added
- Frontend Vercel config: `frontend/vercel.json`
- Frontend nginx config: `frontend/nginx.conf`
- Render blueprint: `render.yaml`
- CI workflow: `.github/workflows/ci.yml`
- Render deploy workflow: `.github/workflows/deploy-render.yml`
- Vercel deploy workflow: `.github/workflows/deploy-vercel.yml`
- Production env templates: `backend/.env.production.example`, `frontend/.env.production.example`

### 1. Supabase Setup
1. Create Supabase project.
2. Get connection string from Project Settings > Database.
3. Use pooler connection string in Render env var `ConnectionStrings__DefaultConnection`.

### 2. Backend Deploy on Render
1. Create a new Web Service from repo.
2. Use `render.yaml` or set manually:
   - Root directory: `backend/src/LotterySystem.API`
   - Environment: `Docker`
   - Dockerfile: `backend/src/LotterySystem.API/Dockerfile`
3. Add environment variables from `backend/.env.production.example`.
4. Add `RENDER_DEPLOY_HOOK` to GitHub Secrets for automated deploy workflow.

### 3. Frontend Deploy on Vercel
1. Import repository into Vercel.
2. Set project root to `frontend`.
3. Build command: `npm run build -- --configuration production`.
4. Output directory: `dist/frontend/browser`.
5. Set environment variable:
   - `NG_APP_API_URL=https://<your-render-domain>/api`
6. Add GitHub secrets:
   - `VERCEL_TOKEN`
   - `VERCEL_ORG_ID`
   - `VERCEL_PROJECT_ID`

### 4. GitHub Actions CI/CD
- `ci.yml` runs build validation for backend and frontend on push/PR.
- `deploy-render.yml` triggers Render deploy hook on main branch.
- `deploy-vercel.yml` builds and deploys Angular frontend to Vercel on main branch.

### 5. Security and Performance Notes
- Never store production secrets in source control.
- Keep `Database__EnableSensitiveDataLogging=false` in production.
- Use Supabase pooler endpoint and SSL (`SSL Mode=Require`).
- Rotate JWT key regularly.
- Enable HTTPS-only cookies if moving refresh tokens from local storage.
