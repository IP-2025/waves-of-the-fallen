import { Entity, PrimaryGeneratedColumn, Column, OneToOne, JoinColumn } from 'typeorm';
import { Credential } from './Credential';

@Entity()
export class Player {
  @PrimaryGeneratedColumn('uuid')
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
