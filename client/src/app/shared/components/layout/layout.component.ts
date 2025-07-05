import { Component, ChangeDetectionStrategy } from "@angular/core";
import { RouterOutlet, RouterLink, RouterLinkActive } from "@angular/router";


@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent {
  // Component logic can be added here
}