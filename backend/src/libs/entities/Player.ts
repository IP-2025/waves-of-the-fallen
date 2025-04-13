import { Entity,  Column, OneToOne, JoinColumn, PrimaryColumn } from 'typeorm';
import { Credential } from './Credential';

@Entity()
export class Player {
  @PrimaryColumn({
      type: 'varchar',
      nullable: false,
      unique: true
    })
  player_id!: string;

  @Column({
    type: 'varchar',
    nullable: true,
    unique: true
  })
  username!: string;

  @Column({ type: 'int', default: 0 })
  gold: number = 0;

  @OneToOne(() => Credential, credential => credential.player)
  @JoinColumn()
  credential!: Credential;
}
