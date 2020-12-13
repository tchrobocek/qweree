import {Component, Input} from '@angular/core';
import {Router} from '@angular/router';

@Component({
  selector: 'app-link',
  templateUrl: './link.component.html',
  styleUrls: ['./link.component.scss']
})
export class LinkComponent {

  @Input() public href: string;
  @Input() public text: string;

  constructor(
    public router: Router
  ) { }

  goto($event): void {
    if (this.href.startsWith('/') || this.href.startsWith('.')) {
      $event.preventDefault();
      this.router.navigate([this.href]);
    }
    else {
      window.location.href = this.href;
    }
  }
}
