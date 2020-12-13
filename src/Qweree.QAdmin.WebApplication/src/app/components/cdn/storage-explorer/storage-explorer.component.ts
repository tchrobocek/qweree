import {Component, OnInit} from '@angular/core';
import {CdnAdapterService} from '../../../services/cdn/cdn-adapter.service';
import {Router} from '@angular/router';

@Component({
  selector: 'app-storage-explorer',
  templateUrl: './storage-explorer.component.html',
  styleUrls: ['./storage-explorer.component.scss']
})
export class StorageExplorerComponent implements OnInit {

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
    this.currentPath = this.inputPath;
  }

  pathChanged(path: string): void {
    if (path === '') {
      path = '/';
    }
    this.currentPath = path;
    this.inputPath = path;
  }

  ngOnInit(): void {
    this.router.events.subscribe(() => {
      this.refreshPath();
    });

    this.refreshPath();
  }

  refreshPath(): void {

    this.currentPath = this.router.url.substring('/cdn/explorer'.length);

    if (!this.currentPath) {
      this.currentPath = '/';
    }

    this.inputPath = this.currentPath.toString();
  }
}
