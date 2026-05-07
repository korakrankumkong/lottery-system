import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  { path: 'auth/login', loadComponent: () => import('./features/auth/login.component').then(m => m.LoginComponent) },
  { path: 'dashboard', canActivate: [authGuard], data: { roles: ['Admin', 'Staff'] }, loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
  { path: 'ticket-entry', canActivate: [authGuard], data: { roles: ['Admin', 'Staff'] }, loadComponent: () => import('./features/ticket-entry/ticket-entry.component').then(m => m.TicketEntryComponent) },
  { path: 'ticket-history', canActivate: [authGuard], data: { roles: ['Admin', 'Staff'] }, loadComponent: () => import('./features/ticket-history/ticket-history.component').then(m => m.TicketHistoryComponent) },
  { path: 'rounds', canActivate: [authGuard], data: { roles: ['Admin', 'Staff'] }, loadComponent: () => import('./features/rounds/rounds.component').then(m => m.RoundsComponent) },
  { path: 'results', canActivate: [authGuard], data: { roles: ['Admin', 'Staff'] }, loadComponent: () => import('./features/results/results.component').then(m => m.ResultsComponent) },
  { path: 'reports', canActivate: [authGuard], data: { roles: ['Admin'] }, loadComponent: () => import('./features/reports/reports.component').then(m => m.ReportsComponent) },
  { path: 'settings', canActivate: [authGuard], data: { roles: ['Admin', 'Staff'] }, loadComponent: () => import('./features/settings/settings.component').then(m => m.SettingsComponent) },
  { path: '**', redirectTo: 'dashboard' }
];
