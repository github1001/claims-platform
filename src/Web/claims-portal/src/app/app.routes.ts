import { Routes } from '@angular/router';

import { Dashboard } from './pages/dashboard/dashboard';
import { SubmitClaim } from './pages/submit-claim/submit-claim';
import { TrackClaim } from './pages/track-claim/track-claim';
import { OfficerWorkspace } from './pages/officer-workspace/officer-workspace';

export const routes: Routes = [
  {
    path: '',
    component: Dashboard
  },
  {
    path: 'submit',
    component: SubmitClaim
  },
  {
    path: 'track',
    component: TrackClaim
  },
  {
    path: 'officer',
    component: OfficerWorkspace
  },
  {
    path: '**',
    redirectTo: ''
  }
];