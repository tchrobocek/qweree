import { Component, OnInit } from '@angular/core';
import {CdnAdapterService} from '../../../services/cdn/cdn-adapter.service';
import {ExplorerObject} from '../../../model/cdn/ExplorerObject';
import {Router} from '@angular/router';

@Component({
  selector: 'app-storage-explorer',
  templateUrl: './storage-explorer.component.html',
  styleUrls: ['./storage-explorer.component.scss']
})
export class StorageExplorerComponent implements OnInit {

  public currentPathObjects: ExplorerObject[];
  public currentPath: string;

  constructor(
    private cdnAdapter: CdnAdapterService,
    private router: Router
  ) {
    this.currentPathObjects = [];
  }

  ngOnInit(): void {
    this.currentPath = this.router.url.substring('/cdn/explorer'.length);

    if (!this.currentPath) {
      this.currentPath = '/';
    }
  }

}
