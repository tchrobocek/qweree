import {Component, OnChanges, SimpleChanges} from '@angular/core';
import {CdnAdapterService} from '../../../services/cdn/cdn-adapter.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-storage-explorer',
  templateUrl: './storage-explorer.component.html',
  styleUrls: ['./storage-explorer.component.scss']
})
export class StorageExplorerComponent implements OnChanges {

  public currentPath: string;
  public inputPath: string;

  constructor(
    private cdnAdapter: CdnAdapterService,
    private router: Router
  ) {
  }

  resetInput(): void {
    this.inputPath = this.currentPath;
  }

  goto(): void {
    window.location.href = '/cdn/explorer' + this.inputPath;
  }

  pathChanged(path: string): void {
    this.currentPath = path;
  }

  ngOnChanges(model: SimpleChanges): void {
    console.log('change explorer');
    console.log(model);
    this.currentPath = this.router.url.substring('/cdn/explorer'.length);

    if (!this.currentPath) {
      this.currentPath = '/';
    }

    this.inputPath = this.currentPath.toString();
  }
}
