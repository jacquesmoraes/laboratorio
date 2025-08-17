import { Routes } from "@angular/router";
import { ShadeListComponent } from "./components/shade/shade-list.component/shade-list.component";

// client/src/app/features/production/shade.routes.ts
export const SHADE_ROUTES: Routes = [
  { path: '', component: ShadeListComponent, title: 'Cores' },
];